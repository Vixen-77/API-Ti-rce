using Microsoft.AspNetCore.Mvc;
using WEBAPPP.Services;
using LibrarySSMS.Models;
using Microsoft.AspNetCore.Cors;
using LibrarySSMS;
using Microsoft.EntityFrameworkCore;
using WEBAPPP.DTOs;

namespace WEBAPPP.Controllers
{
    [ApiController]
    [Route("api/Objectconnexion")]
    public class SmartwatchNewGenController : ControllerBase
    {
        private readonly SmartwatchNewGenService _smartwatchNewGenService;
        private readonly AppDbContext _context;

        public SmartwatchNewGenController(SmartwatchNewGenService smartwatchNewGenService, AppDbContext context)
        {
            _smartwatchNewGenService = smartwatchNewGenService;
            _context = context;
        }

        [HttpPost("createSmartwatchNewGen")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> CreateSmartwatchNewGen([FromForm] string idPatientt)
        {
            var idPatient = Guid.Parse(idPatientt);
            if (idPatient == Guid.Empty)
            {
                return BadRequest("Identifiant du patient invalide.");
            }
            var patient = await _context.Patientss.FirstOrDefaultAsync(m => m.UID == idPatient);
        if (patient == null)
        {
            return NotFound("Patient non trouvé.");
        }
            var smartwatchNG = await _context.SmartwatchNewGens.FirstOrDefaultAsync(m => m.idporteur == idPatient);
            if (smartwatchNG == null)
            {
                try
                {
                    var newSmartwatchNG = await _smartwatchNewGenService.CreateSmartwatchNewGenAsync(idPatient);
                    return Ok(newSmartwatchNG);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erreur serveur : {ex.Message}");
                }
            }
            else
            {
                smartwatchNG.IsConnected = true;
                await _context.SaveChangesAsync();
                return Ok(smartwatchNG);
            }
        }


        [HttpPost("disconnectSmartwatchNewGen")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> DisconnectSmartwatchNewGen([FromForm] string idPatientt)
        {
            var idPatient = Guid.Parse(idPatientt);
            if (idPatient == Guid.Empty)
            {
                return BadRequest("Identifiant du patient invalide.");
            }
            var patient = await _context.Patientss.FirstOrDefaultAsync(m => m.UID == idPatient);
        if (patient == null)
        {
            return NotFound("Patient non trouvé.");
        }
            var smartwatchNG = await _context.SmartwatchNewGens.FirstOrDefaultAsync(m => m.idporteur == idPatient);
            if (smartwatchNG == null)
            {
                return NotFound("Smartwatch non trouvé pour ce patient.");
            }
            else
            {
                smartwatchNG.IsConnected = false;
                await _context.SaveChangesAsync();
                return Ok("Smartwatch déconnecté avec succès.");
            }
        }

        [HttpPost("getstateSmartwatchNewGen")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> GetStateSmartwatchNewGen([FromForm] string idPatientt)
        {
            var idPatient = Guid.Parse(idPatientt);
            if (idPatient == Guid.Empty)
            {
                return BadRequest("Identifiant du patient invalide.");
            }
            var patient = await _context.Patientss.FirstOrDefaultAsync(m => m.UID == idPatient);
        if (patient == null)
        {
            return NotFound("Patient non trouvé.");
        }
            var smartwatchNG = await _context.SmartwatchNewGens.FirstOrDefaultAsync(m => m.idporteur == idPatient);
            if (smartwatchNG == null)
            {
                return Ok("Smartwatch non trouvé pour ce patient.");
            }
            else
            {
                var obj = new EtatOBJ
                {
                    isconnected = smartwatchNG.IsConnected,
                    adrmac = smartwatchNG.ADRMAC,
                    adrip = smartwatchNG.IpAdress
                };
                return Ok(obj);
            }
        }

    }
}
