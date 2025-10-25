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
        public IActionResult GetPlayerInfo([FromQuery] string userId)
        {
            try
            {
                PlayerDto playerDto = _service.GetPlayerInfo(userId);
                
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

        public record SetNicknameRequest(string userId, string newNickname);

        [HttpPost("set-nickname")]
        public IActionResult SetNickname([FromBody] SetNicknameRequest req)
        {
            try
            {
                var playerDto = _service.SetNickname(req.userId, req.newNickname);

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
