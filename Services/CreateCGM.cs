using LibrarySSMS;
using LibrarySSMS.Enums;
using LibrarySSMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using WEBAPPP.DTOs;



namespace WEBAPPP.Services;

    public class CGMService{


         private readonly AppDbContext _context;

        public CGMService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreationOBJ> CreateCGMAsync(Guid idPatient)
        {
            var newCGM = new CGM
            {
                IdCGM = Guid.NewGuid(),
                idporteur = idPatient,
                ADRMAC = GenerateMacAddress(idPatient),
                isGlyhigh = false,
                isGlyLow= false,
                Glycemia = 0,
                battry = 1

            };

            _context.CGMs.Add(newCGM);
            await _context.SaveChangesAsync();

            return new CreationOBJ {
                
                IdOBJEnvoi = newCGM.IdCGM.ToString(),
                AdrMac = newCGM.ADRMAC
            };
        }
        private string GenerateMacAddress(Guid id)
        {
            // On utilise le Guid pour générer une pseudo adresse MAC unique
            var bytes = id.ToByteArray();
            return string.Join(":", bytes.Take(6).Select(b => b.ToString("X2")));
        }


    }




    
