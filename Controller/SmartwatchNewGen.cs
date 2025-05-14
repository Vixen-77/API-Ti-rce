using Microsoft.AspNetCore.Mvc;
using WEBAPPP.Services;
using LibrarySSMS.Models;

namespace WEBAPPP.Controllers
{
    [ApiController]
    [Route("api/Ojectconnection")]
    public class SmartwatchNewGenController : ControllerBase
    {
        private readonly SmartwatchNewGenService _smartwatchNewGenService;

        public SmartwatchNewGenController(SmartwatchNewGenService smartwatchNewGenService)
        {
            _smartwatchNewGenService = smartwatchNewGenService;
        }

        [HttpPost("createSmartwatchNewGen")]
        public async Task<IActionResult> CreateSmartwatchNewGen([FromBody] Guid idPatient)
        {
            try
            {
                var newSmartwatch = await _smartwatchNewGenService.CreateSmartwatchNewGenAsync(idPatient);
                return Ok(newSmartwatch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }
    }
}
