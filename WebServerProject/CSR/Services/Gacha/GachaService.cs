using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;
using WebServerProject.CSR.Repositories.Character;
using WebServerProject.CSR.Repositories.Gacha;
using WebServerProject.CSR.Repositories.User;
using WebServerProject.CSR.Services.Gacha;
using WebServerProject.Models.DTOs.Gacha;
using WebServerProject.Models.DTOs.UserEntity;
using WebServerProject.Models.Entities.CharacterEntity;
using WebServerProject.Models.Entities.Gacha;
using WebServerProject.Models.Entities.GachaEntity;
using WebServerProject.Models.Entities.UserEntity;

namespace WebServerProject.CSR.Services
{
    public interface IGachaService 
    {
        public Task<List<GachaMasterDTO>?> GetGachaListAsync();

        public Task<GachaDrawResultDTO> DrawAsync(string gachaId, int userId);
    }

    public class GachaService : IGachaService
    {
        public readonly IUserRepository _userRepository;
        public readonly ICharacterRepository _characterRepository;
        public readonly IGachaRepository _gachaRepository;
        public readonly IGachaRandomizer _gachaRandomizer;

        private readonly string _connectionString;

        public GachaService(
            IUserRepository userRepository,
            ICharacterRepository characterRepository,
            IGachaRepository gachaRepository,
            IGachaRandomizer gachaRandomizer,
            IConfiguration config)
        {
            _userRepository = userRepository;   
            _characterRepository = characterRepository;
            _gachaRepository = gachaRepository;
            _gachaRandomizer = gachaRandomizer;

            _connectionString = config.GetConnectionString("GameDb")
            ?? throw new InvalidOperationException("ConnectionStrings:GameDb is missing.");
        }

        public async Task<List<GachaMasterDTO>?> GetGachaListAsync()
        {
            var gachaMasterDTOList = new List<GachaMasterDTO>();

            var gachaMasterList = await _gachaRepository.GetGachaListAsync();
            if(gachaMasterList == null)
            {
                throw new InvalidOperationException("가챠 목록 데이터를 불러올 수 없습니다.");
            }

            foreach (var gachaMasger in gachaMasterList)
            {
                gachaMasterDTOList.Add(GachaMasterDTO.FromGachaMaster(gachaMasger));
            }
            if(gachaMasterDTOList.Count == 0)
            {
                throw new InvalidOperationException("등록된 가챠가 없습니다.");
            }

            return gachaMasterDTOList;
        }

        public async Task<GachaDrawResultDTO> DrawAsync(string gachaId, int userId)
        {
            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var db = new QueryFactory(conn, new MySqlCompiler());

            // userId 확인
            var user = await _userRepository.GetUserByIdAsync(userId, db);
            if (user == null)
            {
                return new GachaDrawResultDTO
                {
                    Success = false,
                    Message = "유효하지 않은 사용자입니다."
                };
            }

            await using var tx = await conn.BeginTransactionAsync();
            bool committed = false;

            try
            {
                // 뽑기 마스터 정보 조회
                var gachaMaster = await _gachaRepository.GetGachaAsync(gachaId, db, tx);
                if (gachaMaster == null)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "유효하지 않은 가챠입니다."
                    };
                }

                // 재화 확인
                var resource = await _userRepository.GetUserResourcesForUpdateAsync(userId, db, tx);
                if (resource == null)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "유저 재화 정보를 찾을 수 없습니다."
                    };
                }
                else if (resource.diamond < 100)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "다이아몬드가 부족합니다. 남은 다이아 : " + resource.diamond,
                        RemainingResources = UserResourcesDTO.FromUserResources(resource)
                    };
                }

                // 재화 소모
                UserResources userResources = new UserResources
                {
                    gold = resource.gold,
                    diamond = resource.diamond - 100
                };
                var updateResourcesResult = await _userRepository.UpdateResourcesAsync(user.id, userResources, db, tx);
                if (updateResourcesResult == false)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "재화 업데이트에 실패했습니다."
                    };
                }

                // 유저의 천장 스택 조회
                UserGachaPity pityData = await _gachaRepository.GetUserGachaPityStackForUpdateAsync(userId, gachaMaster.id, db, tx);
                int currentPityStack = pityData != null ? pityData.pity_stack : 0; // 첫 뽑기인 경우 0으로 초기화

                // 뽑기 로직
                var selectedItem = await _gachaRandomizer.SelectItemAsync(gachaMaster, currentPityStack, db, tx);
                if (selectedItem == null)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "가챠 아이템 선택에 실패했습니다."
                    };
                }

                // 뽑기 결과에 따른 스택 갱신
                int newPityStack = (selectedItem.rarity >= gachaMaster.pity_target_rarity) ? 0 : currentPityStack + 1;
                // 계산된 스택을 DB에 저장
                await _gachaRepository.UpsertUserGachaPityStackAsync(userId, gachaMaster.id, newPityStack, db, tx);

                // 보상 지급
                var result = await GrantGachaRewardAsync(userId, selectedItem, db, tx);
                if (result == null)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "가챠 보상 지급에 실패했습니다."
                    };
                }
                if (!result.Success)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = result.Message
                    };
                }

                await tx.CommitAsync();
                committed = true;

                // 결과 반환
                return new GachaDrawResultDTO
                {
                    Success = true,
                    Message = result.Message,
                    isNew = result.IsNew,

                    DrawnItem = GachaPoolDTO.FromGachaPool(selectedItem),
                    RemainingResources = new UserResourcesDTO
                    {
                        Gold = resource.gold,
                        Diamond = resource.diamond - 100,
                    }
                };
            }
            finally
            {
                if(committed == false)
                {
                    try { await tx.RollbackAsync(); } catch { }
                }
            }
        }

        // 보상 지급 처리
        private async Task<GachaRewardResultDTO> GrantGachaRewardAsync(int userId, GachaPool poolItem, QueryFactory? db = null, IDbTransaction? tx = null)
        {
            var result = new GachaRewardResultDTO();
            result.Success = true;
            result.gachaPool = GachaPoolDTO.FromGachaPool(poolItem);

            switch (poolItem.item_type)
            {
                case (int)GachaPool.ItemType.ITEM_CHARACTER:
                    // 캐릭터 획득 처리
                    // 중복 확인
                    UserCharacter userCharacter =  await _characterRepository.GetUserCharacterAsync(userId, poolItem.item_id, db, tx);
                    
                    // 중복된 캐릭터 획득 
                    if(userCharacter != null)
                    {
                        result.Message = "중복된 캐릭터 입니다.";
                        result.IsNew = true;

                        // 중복 보상으로 대체 지급
                        //
                    }
                    else
                    {
                        result.Message = "새로운 캐릭터 입니다.";
                        result.IsNew = false;

                        // 기존 보상 지급
                        var addCharacerResult = await _characterRepository.AddCharacterToUserAsync(userId, poolItem.item_id, db, tx);
                        if (addCharacerResult == 0)
                        {
                            throw new Exception("뽑기 결과 저장 중 오류가 발생했습니다.");
                        }
                    }

                    break;

                case (int)GachaPool.ItemType.ITEM_EQUIPMENT:
                    // 장비 획득 처리
                    
                    break;

                case (int)GachaPool.ItemType.ITEM_CONSUMABLE:
                    // 소비 아이템 획득 처리
                    
                    break;

                default:
                    throw new NotSupportedException($"지원하지 않는 아이템 타입입니다. (item_type: {poolItem.item_type})");
            }

            return result;
        }
    }
}
