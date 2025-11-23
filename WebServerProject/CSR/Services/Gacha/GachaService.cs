using WebServerProject.CSR.Repositories.Character;
using WebServerProject.CSR.Repositories.Gacha;
using WebServerProject.CSR.Repositories.User;
using WebServerProject.CSR.Services.Gacha;
using WebServerProject.Models.DTOs.Gacha;
using WebServerProject.Models.DTOs.UserEntity;
using WebServerProject.Models.Entities.CharacterEntity;
using WebServerProject.Models.Entities.GachaEntity;

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

        public GachaService(
            IUserRepository userRepository,
            ICharacterRepository characterRepository,
            IGachaRepository gachaRepository,
            IGachaRandomizer gachaRandomizer)
        {
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
                throw new InvalidOperationException("사용자를 찾을 수 없습니다.");
            }

            // 재화 확인
            var resource = await _userRepository.GetUserResourcesByIdAsync(userId);
            if (resource == null)
            {
                throw new InvalidOperationException("유저 재화 정보를 찾을 수 없습니다.");
            }
            else if(resource.diamond < 100)
            {
                return new GachaDrawResultDTO
                {
                    Success = false,
                    Message = "다이아몬드가 부족합니다. 남은 다이아 : " + resource.diamond,
                    RemainingResources = UserResourcesDTO.FromUserResources(resource)
                };
            }

            // 뽑기 로직
            var selectedItem = await _gachaRandomizer.SelectItemAsync(gachaId);
            if (selectedItem == null)
            {
                throw new InvalidOperationException("가챠 아이템 선택에 실패했습니다.");
            }

            // 결과 저장 (트랜잭션 처리 필요) 
            // 재화 소모
            //

            // 보상 지급
            var result = await GrantGachaRewardAsync(userId, selectedItem);
            if(result == null)
            {
                throw new InvalidOperationException("가챠 보상 지급에 실패했습니다.");
            }
            if (!result.Success)
            {
                return new GachaDrawResultDTO
                {
                    Success = false,
                    Message = result.Message
                };
            }

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

        // 보상 지급 처리
        private async Task<GachaRewardResultDTO> GrantGachaRewardAsync(int userId, GachaPool poolItem)
        {
            var result = new GachaRewardResultDTO();
            result.Success = true;
            result.gachaPool = GachaPoolDTO.FromGachaPool(poolItem);

            switch (poolItem.item_type)
            {
                case (int)GachaPool.ItemType.ITEM_CHARACTER:
                    // 캐릭터 획득 처리
                    // 중복 확인
                    UserCharacter userCharacter =  await _characterRepository.GetUserCharacterAsync(userId, poolItem.item_id);
                    
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
                        var addCharacerResult = await _characterRepository.AddCharacterToUserAsync(userId, poolItem.item_id);
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
