using Microsoft.AspNetCore.Authorization;
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
    public class CGMController : ControllerBase
    {
        private readonly SmartwatchService _smartwatchService;
        private readonly CGMService _cgmService;
        private readonly AppDbContext _context;

        public CGMController(SmartwatchService smartwatchService, AppDbContext context, CGMService cgmService)
        {
            _cgmService = cgmService;
            _smartwatchService = smartwatchService;
            _context = context;
        }

        [HttpPost("createCGM")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> CreateGGM([FromForm] string idPatientt)
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
            var CGM = await _context.CGMs.FirstOrDefaultAsync(m => m.idporteur == idPatient);
            if (CGM == null)
            {

                try
                {
                    var CGMnew = await _cgmService.CreateCGMAsync(idPatient);
                    return Ok(CGMnew);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erreur lors de la création du CGM : {ex.Message}");
                }
            }
            else
            {
                CGM.IsConnected = true;
                await _context.SaveChangesAsync();
                return Ok(CGM);
            }
        }

        [HttpPost("disconnectCGM")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> DisconnectCGM([FromForm] string idPatientt)
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
            var CGM = await _context.CGMs.FirstOrDefaultAsync(m => m.idporteur == idPatient);
            if (CGM == null)
            {
                return NotFound("CGM non trouvé pour ce patient.");
            }
            else
            {
                CGM.IsConnected = false;
                await _context.SaveChangesAsync();
                return Ok("CGM déconnecté avec succès.");
            }
        }

        [HttpPost("getstateCGM")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> GetStateCGM([FromForm] string idPatientt)
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
            var CGM = await _context.CGMs.FirstOrDefaultAsync(m => m.idporteur == idPatient);
            if (CGM == null)
            {
                return Ok("CGM non trouvé pour ce patient.");
            }
            else
            {   
                var obj = new EtatOBJ
                {
                    isconnected = CGM.IsConnected,
                    adrmac = CGM.ADRMAC,
                    adrip = "",
                };
                return Ok(obj);
            }
        }
    }
}
