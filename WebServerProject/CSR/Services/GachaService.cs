using System.Reflection.Metadata.Ecma335;
using WebServerProject.CSR.Repositories;
using WebServerProject.Models.DTOs.Gacha;
using WebServerProject.Models.DTOs.User;
using WebServerProject.Models.Entities.Gacha;
using WebServerProject.Models.Gacha;

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
            IGachaRepository gachaRepository,
            IGachaRandomizer gachaRandomizer)
        {
            _userRepository = userRepository;   
            _gachaRepository = gachaRepository;
            _gachaRandomizer = gachaRandomizer;
        }

        public async Task<List<GachaMasterDTO>?> GetGachaListAsync()
        {
            var gachaMasterDTOList = new List<GachaMasterDTO>();

            var gachaMasterList = await _gachaRepository.GetGachaListAsync();
            if(gachaMasterDTOList == null)
            {
                return null;
            }

            foreach (var gachaMaster in gachaMasterList)
            {
                gachaMasterDTOList.Add(GachaMasterDTO.FromGachaMaster(gachaMaster));
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
                    Message = "사용자를 찾을 수 없습니다."
                };
            }

            // 재화 확인
            var resourse = await _userRepository.GetUserResourcesByIdAsync(userId);
            if (resourse == null)
            {
                return new GachaDrawResultDTO
                {
                    Success = false,
                    Message = "유저 정보가 없습니다."
                };
            }
            else if(resourse.diamond < 100)
            {
                return new GachaDrawResultDTO
                {
                    Success = false,
                    Message = "다이아몬드가 부족합니다. 남은 다이아 : " + resourse.diamond,
                    RemainingResources = UserResourcesDTO.FromUserResources(resourse)
                };
            }

            // 뽑기 로직
            var selectedItem = await _gachaRandomizer.SelectItemAsync(gachaId);
            if (selectedItem == null)
            {
                return new GachaDrawResultDTO
                {
                    Success = false,
                    Message = "가챠 선택 중 오류가 발생했습니다."
                };
            }

            // 결과 저장
            var result = await GrantGachaRewardAsync(userId, selectedItem);
            if(result == null)
            {
                return new GachaDrawResultDTO
                {
                    Success = false,
                    Message = "보상 지급 중 오류가 발생했습니다."
                };
            }

            // 결과 반환
            return new GachaDrawResultDTO
            {
                Success = true,
                Message = "뽑기 성공",

                DrawnItem = GachaPoolDTO.FromGachaPool(selectedItem),
                RemainingResources = new Models.DTOs.User.UserResourcesDTO
                {
                    Gold = resourse.gold,
                    Diamond = resourse.diamond - 100,
                }
            };
        }

        // 보상 지급 처리
        private async Task<GachaRewardResultDTO> GrantGachaRewardAsync(int userId, GachaPool poolItem)
        {
            var result = new GachaRewardResultDTO();
            result.ItemType = poolItem.item_type;
            result.ItemId = poolItem.item_id;
            result.Rarity = poolItem.rarity;

            switch (poolItem.item_type)
            {
                case (int)GachaPool.ItemType.ITEM_CHARACTER:
                    // 캐릭터 획득 처리
                    var addCharacterResult = await _characterRepository.AddCharacterToUser(userId, poolItem.item_id);
                    if (addCharacterResult == null)
                    {
                        result.Success = false;
                        result.Message = "캐릭터 추가 중 오류가 발생했습니다.";
                    }
                    result.Success = addCharacterResult.Success;
                    result.Message = addCharacterResult.Message;

                    break;

                case (int)GachaPool.ItemType.ITEM_EQUIPMENT:
                    // 장비 획득 처리
                    
                    break;

                case (int)GachaPool.ItemType.ITEM_CONSUMABLE:
                    // 소비 아이템 획득 처리
                    
                    break;

                default:
                    throw new Exception($"Invalid item type: {poolItem.item_type}");
            }

            return result;
        }
    }
}
