using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEBAPPP.Services; // Ton namespace Service
using LibrarySSMS.Models;

namespace WEBAPPP.Controllers
{
    [ApiController]
    [Route("api/Ojectconnection")]
    public class CGMController : ControllerBase
    {
        private readonly SmartwatchService _smartwatchService;

        public CGMController(SmartwatchService smartwatchService)
        {
            _smartwatchService = smartwatchService;
        }

        [HttpPost("createCGM")]
        [Authorize] // <- tu peux enlever si tu ne veux pas vérifier le JWT
        public async Task<IActionResult> CreateSmartwatch([FromBody] Guid idPatient)
        {
            if (idPatient == Guid.Empty)
            {
                return BadRequest("Identifiant du patient invalide.");
            }

            try
            {
                var smartwatch = await _smartwatchService.CreateSmartwatchAsync(idPatient);
                return Ok(smartwatch);
            }
            catch (Exception ex)
            {
                // Log l'erreur si besoin ici
                return StatusCode(500, $"Erreur lors de la création de la montre : {ex.Message}");
            }
        }
    }
}
