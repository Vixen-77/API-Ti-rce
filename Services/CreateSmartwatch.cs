using LibrarySSMS;
using LibrarySSMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using WEBAPPP.DTOs;


namespace WEBAPPP.Services;

    public class SmartwatchService{


         private readonly AppDbContext _context;

        public SmartwatchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreationOBJ> CreateSmartwatchAsync(Guid idPatient)
        {
            var newSmartwatch = new Smartwatch
            {
                IdSmartwatch = Guid.NewGuid(),
                idporteur = idPatient,
                ADRMAC = GenerateMacAddress(idPatient),
                Marque = "DefaultBrand",
                Modele = "DefaultModel",
                FC_capte = null,
                PAS_capte = null,
                PAD_capte = null,
                TGS_capte = null,
                IsConnected = true,
                Latitude = 0,
                Longitude = 0,
                Timestamp = DateTimeOffset.UtcNow
            };
            var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == idPatient);
            if (patient == null)
            {
                throw new Exception("Patient non trouvé.");
            }
            patient.IdSmartwatchP = newSmartwatch.IdSmartwatch;
            _context.Smartwatches.Add(newSmartwatch);
            await _context.SaveChangesAsync();

            return new CreationOBJ {
                
                IdOBJEnvoi = newSmartwatch.IdSmartwatch.ToString(),
                AdrMac = newSmartwatch.ADRMAC
            };
        }
        private string GenerateMacAddress(Guid id)
        {
            // On utilise le Guid pour générer une pseudo adresse MAC unique
            var bytes = id.ToByteArray();
            return string.Join(":", bytes.Take(6).Select(b => b.ToString("X2")));
        }


    }




    
