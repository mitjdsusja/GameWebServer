using Microsoft.AspNetCore.Mvc;
using WebServerProject.Data;
using WebServerProject.Models.DTOs;
using WebServerProject.Services;

namespace WebServerProject.Contollers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _service;

        public PlayerController(PlayerService service)
        {
            _service = service;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetPlayerInfo([FromQuery] string userId)
        {
            try
            {
                PlayerDto playerDto = await _service.GetPlayerInfoAsync(userId);
                
                return Ok(new
                {
                    playerDto.userId,
                    playerDto.nickname,
                    playerDto.level,
                    playerDto.gold,
                    playerDto.diamonds,
                    playerDto.profileId,
                });
            }
            catch(Exception ex)
            {
                if (ex.Message == "User not found")
                    return NotFound(new { error = ex.Message });
                else
                    return StatusCode(500, new { error = ex.Message });
            }
        }

        public record SetNicknameRequest(string userId, string nickname);

        [HttpPost("set-nickname")]
        public async Task<IActionResult> SetNickname([FromBody] SetNicknameRequest req)
        {
            try
            {
                var playerDto = await _service.SetNicknameAsync(req.userId, req.nickname);

                return Ok(new { playerDto.userId, playerDto.nickname });
            }
            catch (Exception ex)
            { 
                if (ex.Message == "User not found")
                    return NotFound(new { error = ex.Message });
                else
                    return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
