using Microsoft.AspNetCore.Mvc;
using WebServerProject.Data;
using WebServerProject.Models.DTOs;
using WebServerProject.Services;

namespace WebServerProject.Contollers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetPlayerInfo([FromQuery] string userId)
        {
            return Ok(new
            {
                   
            });
        }
    }
}
