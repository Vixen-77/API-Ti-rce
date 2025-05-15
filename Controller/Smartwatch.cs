using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEBAPPP.Services;
using LibrarySSMS.Models;
using Microsoft.AspNetCore.Cors;
using LibrarySSMS;
using Microsoft.EntityFrameworkCore;

namespace WEBAPPP.Controllers
{
    [ApiController]
    [Route("api/Objectconnexion")]
    public class SmartwatchController : ControllerBase
    {
        private readonly SmartwatchService _smartwatchService;
        private readonly AppDbContext _context;

        public SmartwatchController(SmartwatchService smartwatchService, AppDbContext context)
        {
            _context = context;
            _smartwatchService = smartwatchService;
        }

        [HttpPost("createSmartwatch")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> CreateSmartwatch([FromForm] string idPatientt)
        {
            var idPatient = Guid.Parse(idPatientt);
            if (idPatient == Guid.Empty)
            {
                return BadRequest("Identifiant du patient invalide.");
            }
            var smartwatch = await _context.Smartwatches.FirstOrDefaultAsync(m => m.idporteur == idPatient);
            if (smartwatch == null)
            {
                try
                {
                    var smartwatchNEW = await _smartwatchService.CreateSmartwatchAsync(idPatient);
                    return Ok(smartwatchNEW);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erreur lors de la création de la montre : {ex.Message}");
                }
            }
            else
            {
                smartwatch.IsConnected = true;
                await _context.SaveChangesAsync();
                return Ok(smartwatch);
            }
        }


        [HttpPost("disconnectSmartwatch")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> DisconnectSmartwatch([FromForm] string idPatientt)
        {
            var idPatient = Guid.Parse(idPatientt);
            if (idPatient == Guid.Empty)
            {
                return BadRequest("Identifiant du patient invalide.");
            }

            var smartwatch = await _context.Smartwatches.FirstOrDefaultAsync(m => m.idporteur == idPatient);
            if (smartwatch == null)
            {
                return NotFound("Smartwatch non trouvé pour ce patient.");
            }
            else
            {
                smartwatch.IsConnected = false;
                await _context.SaveChangesAsync();
                return Ok("Smartwatch déconnecté avec succès.");
            }
        }

        [HttpPost("getstateSmartwatch")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> GetStateSmartwatch([FromForm] string idPatientt)
        {
            var idPatient = Guid.Parse(idPatientt);
            if (idPatient == Guid.Empty)
            {
                return BadRequest("Identifiant du patient invalide.");
            }

            var smartwatch = await _context.Smartwatches.FirstOrDefaultAsync(m => m.idporteur == idPatient);
            if (smartwatch == null)
            {
                return NotFound("Smartwatch non trouvé pour ce patient.");
            }
            else
            {
                return Ok(smartwatch.IsConnected);
            }
        }


    }
}
