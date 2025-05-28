using Microsoft.AspNetCore.Mvc;
using WEBAPPP.Services;
using LibrarySSMS.Models;
using Microsoft.AspNetCore.Http;
using LibrarySSMS;
using Microsoft.AspNetCore.Cors;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace WEBAPPP.Controllers
{
    [ApiController]
    [Route("api/Melinda")]
    public class IdmelState2 : ControllerBase
    {
    
        private readonly AppDbContext _context;
        
        private readonly IMemoryCache _cache;


        public IdmelState2(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpPost("changerstateAlerte")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> CreateSmartwatchNewGen([FromForm] string idAlerte)
        {
            try
            {   
                var idA = Guid.Parse(idAlerte);
                var alerte = await _context.Alerts.FirstOrDefaultAsync(x => x.AlertID == idA);
                if (alerte == null)
                {
                    return NotFound("Alerte non trouvée.");
                }
                alerte.State = 2;
                var idpatient = alerte.PatientUID.ToString();
                _cache.Remove("alert_" + idpatient.ToLower());
                await _context.SaveChangesAsync();
                return Ok("L'état de l'alerte a été mis à jour avec succès a letat 2 melindaaaaa.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }
    }
}