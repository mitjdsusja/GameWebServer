using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;
using System.Data.Common;
using WebServerProject.CSR.Repositories.Character;
using WebServerProject.CSR.Repositories.Gacha;
using WebServerProject.CSR.Repositories.User;
using WebServerProject.CSR.Services.Gacha;
using WebServerProject.Models.DTOs.Gacha;
using WebServerProject.Models.DTOs.UserEntity;
using WebServerProject.Models.Entities.CharacterEntity;
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
        private readonly QueryFactory _db;
        private readonly IUserRepository _userRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly IGachaRepository _gachaRepository;
        private readonly IGachaRandomizer _gachaRandomizer;

        public GachaService(
            QueryFactory db,
            IUserRepository userRepository,
            ICharacterRepository characterRepository,
            IGachaRepository gachaRepository,
            IGachaRandomizer gachaRandomizer,
            IConfiguration config)
        {
            _db = db;
            _userRepository = userRepository;   
            _characterRepository = characterRepository;
            _gachaRepository = gachaRepository;
            _gachaRandomizer = gachaRandomizer;
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
            // userId 확인
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new GachaDrawResultDTO
                {
                    Success = false,
                    Message = "유효하지 않은 사용자입니다."
                };
            }

            // 뽑기 정보 확인
            var gacha = await _gachaRepository.GetGachaAsync(gachaId);
            if (gacha == null)
            {
                return new GachaDrawResultDTO
                {
                    Success = false,
                    Message = "유효하지 않은 가챠입니다."
                };
            }

            var connection = _db.Connection as DbConnection;
            if (connection == null)
            {
                throw new InvalidOperationException("데이터베이스 연결 객체가 생성되지 않았습니다.");
            }
            if(connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var tx = await connection.BeginTransactionAsync();

            try
            {
                // 재화 확인
                var resource = await _userRepository.GetUserResourcesForUpdateAsync(userId, tx);
                if (resource == null)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "유저 재화 정보를 찾을 수 없습니다."
                    };
                }
                else if (resource.diamond < gacha.cost_amount)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "다이아몬드가 부족합니다. 남은 다이아 : " + resource.diamond,
                        RemainingResources = UserResourcesDTO.FromUserResources(resource)
                    };
                }

                // 뽑기 로직
                var selectedItem = await _gachaRandomizer.SelectItemAsync(gachaId, tx);
                if (selectedItem == null)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "가챠 아이템 선택에 실패했습니다."
                    };
                }

                // 재화 소모
                UserResources userResources = new UserResources
                {
                    gold = resource.gold,
                    diamond = resource.diamond - gacha.cost_amount
                };
                var updateResourcesResult = await _userRepository.UpdateResourcesAsync(user.id, userResources, tx);
                if (updateResourcesResult == false)
                {
                    return new GachaDrawResultDTO
                    {
                        Success = false,
                        Message = "재화 업데이트에 실패했습니다."
                    };
                }

                // 보상 지급
                var result = await GrantGachaRewardAsync(userId, selectedItem, tx);
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
                        Diamond = resource.diamond - gacha.cost_amount,
                    }
                };
            }
            catch (Exception ex)
            {
                try {
                    await tx.RollbackAsync();
                } 
                catch (Exception rollbackEx){ 

                }
                throw;
            }
        }

        // 보상 지급 처리
        private async Task<GachaRewardResultDTO> GrantGachaRewardAsync(int userId, GachaPool poolItem, IDbTransaction? tx = null)
        {
            var result = new GachaRewardResultDTO();
            result.Success = true;
            result.gachaPool = GachaPoolDTO.FromGachaPool(poolItem);

            switch (poolItem.item_type)
            {
                case (int)GachaPool.ItemType.ITEM_CHARACTER:
                    // 캐릭터 획득 처리
                    // 중복 확인
                    UserCharacter userCharacter =  await _characterRepository.GetUserCharacterAsync(userId, poolItem.item_id, tx);
                    
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
                        var addCharacerResult = await _characterRepository.AddCharacterToUserAsync(userId, poolItem.item_id, tx);
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
