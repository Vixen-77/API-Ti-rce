using LibrarySSMS;
using LibrarySSMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using WEBAPPP.DTOs;

namespace WEBAPPP.Services
{
    public class SmartwatchNewGenService
    {
        private readonly AppDbContext _context;

        public SmartwatchNewGenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreationOBJ> CreateSmartwatchNewGenAsync(Guid idPatient)
        {
            var newSmartwatch = new SmartwatchNewGen
            {
                IdSmartwatchNewGen = Guid.NewGuid(),
                idporteur = idPatient,
                ADRMAC = GenerateMacAddress(idPatient),
                Marque = "DefaultBrandNewGen",
                Modele = "DefaultModelNewGen",
                Heart_Rate = null,
                IpAdress= GenerateIPAddress(idPatient),
                Respiratory_Rate = null,
                Body_Temperature = null,
                Oxygen_Saturation = null,
                Systolic_Blood_Pressure = null,
                Diastolic_Blood_Pressure = null,
                Derived_HRV = null,
                Derived_Pulse_Pressure = null,
                Derived_BMI = null,
                Derived_MAP = null,
                IsConnected = true,
                Latitude = 0,
                Longitude = 0,
                Timestamp = DateTimeOffset.UtcNow,
              //  IpAdress = "0.0.0.0" // par défaut // a melinda de faire sa 
            };
            var patient = await _context.Patientss.FirstOrDefaultAsync(p => p.UID == idPatient);
            if (patient == null)
            {
            throw new Exception("Patient non trouvé.");
            }
            patient.IdSmartwatchNewGenP = newSmartwatch.IdSmartwatchNewGen;
            _context.SmartwatchNewGens.Add(newSmartwatch);
            await _context.SaveChangesAsync();
            return new CreationOBJ{
                
                IdOBJEnvoi = newSmartwatch.IdSmartwatchNewGen.ToString(),
                AdrMac = newSmartwatch.ADRMAC
                //rajouter IP
            };
        }

        private string GenerateMacAddress(Guid id)
        {
            var bytes = id.ToByteArray();
            return string.Join(":", bytes.Take(6).Select(b => b.ToString("X2")));
        }

        private string GenerateIPAddress(Guid id)
       {
         var bytes = id.ToByteArray();

         // Prendre les 4 premiers octets pour faire 4 nombres
          var part1 = bytes[0];
          var part2 = bytes[1];
          var part3 = bytes[2];
          var part4 = bytes[3];

         // Pour éviter les IP réservées (0.0.0.0, 255.255.255.255...), on ajuste un peu :
         part1 = (byte)(part1 % 223 + 1); // 1 à 223
         part2 = (byte)(part2 % 256);     // 0 à 255
         part3 = (byte)(part3 % 256);     // 0 à 255
         part4 = (byte)(part4 % 254 + 1); // 1 à 254 (éviter 0 et 255)

         return $"{part1}.{part2}.{part3}.{part4}";
}

    }



}
