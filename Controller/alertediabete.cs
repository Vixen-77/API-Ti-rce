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
    [Route("api/alertediabete")]
    public class AlerteDiab : ControllerBase
    {
        private readonly SmartwatchService _smartwatchService;
        private readonly CGMService _cgmService;
        private readonly AppDbContext _context;

        public AlerteDiab(SmartwatchService smartwatchService, AppDbContext context, CGMService cgmService)
        {
            _cgmService = cgmService;
            _smartwatchService = smartwatchService;
            _context = context;
        }

        [HttpPost("AlerteDiab")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> AlerteDiabet([FromForm] string raison, [FromForm] string idPatientt, [FromForm] string latitude, [FromForm] string longitude)
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
            var alertexistante = await _context.Alerts.FirstOrDefaultAsync(m => m.PatientUID == idPatient && (m.State == 0 || m.State == 1));
            if (alertexistante != null)
            {
                return Ok("Une alerte est déjà en cours pour ce patient.");
            }
            if (raison == "hypo")
            {
                var alert = new Alert
                {
                    AlertID = Guid.NewGuid(),
                    PatientUID = idPatient,
                    CreatedAt = DateTime.Now,
                    State = 0,
                    Descrip = "Hypoglycemia",
                    Color = "orange",
                    latitudePatient = latitude,
                    longitudePatient = longitude
                };

                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();
                return Ok("Alerte de diabète créée avec succès.");
            }
            else
            if (raison == "hyper")
            {
                var alert = new Alert
                {
                    AlertID = Guid.NewGuid(),
                    PatientUID = idPatient,
                    CreatedAt = DateTime.Now,
                    State = 0,
                    Descrip = "Hyperglycemia",
                    Color = "orange",
                    latitudePatient = latitude,
                    longitudePatient = longitude
                };

                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();
                return Ok("Alerte de diabète créée avec succès.");
            }
            else
            {
                return BadRequest("Raison d'alerte invalide.");
            }

        }

    }
}
