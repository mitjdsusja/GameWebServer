using Microsoft.AspNetCore.Mvc;
using WebServerProject.Models.DTOs.Request;
using WebServerProject.Models.DTOs.Response;
using WebServerProject.Services;

namespace WebServerProject.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GachaController : ControllerBase
    {
        private readonly IGachaService _gachaService;

        public GachaController(IGachaService gachaService )
        {
            _gachaService = gachaService;
        }

        [HttpPost("draw")]
        public async Task<GachaResponse> Draw([FromBody] GachaRequest req)
        {
            var result = await _gachaService.DrawAsync(req.gachaId, req.userId);

            return new GachaResponse
            {

            };
        }
    }
}
