using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEBAPPP.DTOs;
using LibrarySSMS;
using LibrarySSMS.Models;
using WEBAPPP.Interface;

namespace WEBAPPP.Services
{
    public interface IAnomalyDetectionServiceMel
    {
        Task<Alert?> AnalyseSmartwatchData(
            string IdPat,
            bool start,
            string longitude,
            string latitude);
    }
    
    public class AnomalyDetectionServiceMel : IAnomalyDetectionServiceMel
    {
        private readonly string _modelPath;
        private readonly AppDbContext _context;
        private readonly float _alpha = 0.12f;
        private float _p = 0f;
        private readonly ISmsService _smsService;
        private readonly string _emergencyPhone;
         private static readonly float[] inputData= new float[]
    {
      0.03852231318470692f,0.6612016733818098f,1.270525274036f,-0.6677882166468541f,1.449768042309952f,-0.9551905860054939f,-1.1283502425341279f,1f,0.24230647809282194f,-1.4418820513236872f,1.1933414190577327f,1.7347162537307297f,1.245813148601108f,0.1086624568262817f
    };


        public AnomalyDetectionServiceMel(
            IConfiguration configuration,
            AppDbContext context,
            ISmsService smsService)
        {
            _smsService = smsService;
            _context = context;
            _modelPath = configuration["MLModelPath"]
                         ?? throw new ArgumentException("MLModelPath manquant dans la configuration.");
            _emergencyPhone = configuration["EmergencyPhoneNumber"]
                              ?? throw new ArgumentException("EmergencyPhoneNumber manquant dans la configuration.");
        }

        public async Task<Alert?> AnalyseSmartwatchData(
            string IdPat, //FIXME: idpatient
            bool start,
            string longitude,
            string latitude)
        {
            if (!start)
            {
                Console.WriteLine("⛔ Simulation arrêtée.");
                return null;
            }

            if (inputData == null || inputData.Length != 14)
            {
                throw new ArgumentException("Les données d'entrée doivent contenir exactement 14 éléments.");
            }

            var UIDPat = Guid.Parse(IdPat);
            var patient = await _context.Patientss
                .FirstOrDefaultAsync(s => s.UID == UIDPat);

            if (patient == null)
            {
                Console.WriteLine("Patient non trouvée.");
                return null;
            }

            var alertexistante = await _context.Alerts.FirstOrDefaultAsync(m => m.PatientUID == UIDPat && (m.State == 0 || m.State == 1));
                if ( alertexistante != null && alertexistante.Color == "rouge")
                {
                    Console.WriteLine("Une alerte est déjà en cours pour ce patient.");
                    return null;
                }
            if (alertexistante != null && alertexistante.Color == "orange")
            {
                _context.Alerts.Remove(alertexistante);
                await _context.SaveChangesAsync();
            }

            string nomPatient = $"{patient.Name} {patient.LastName}";

            Console.WriteLine(". Début de l'analyse...");
            while (true)
            {   
                try
                {   
                    using (var session = new InferenceSession(_modelPath))
                    {
                        Console.WriteLine("model chargé");
                        
                        var inputTensor = new DenseTensor<float>(inputData, new[] { 1, 14 });
                        var inputs = new List<NamedOnnxValue>
                        {
                            NamedOnnxValue.CreateFromTensor("input", inputTensor)
                        };

                        using var results = session.Run(inputs);
                        float prediction = results.First().AsEnumerable<float>().First();

                        _p = _alpha * prediction + (1 - _alpha) * _p;
                        Console.WriteLine($"📊 Prédiction brute : {prediction}, valeur lissée : {_p:F4}");

                        if (_p < 0.5f)
                        {
                            Console.WriteLine("✅ Aucune anomalie détectée.");
                        }
                        /*else if (_p < 0.7f)
                        {
                            Console.WriteLine("⚠️ Avertissement : Anomalie détectée (niveau moyen)");
                            patient.StateColor = 1;
                            await _context.SaveChangesAsync();
                            return null;
                        }*/
                        else
                        {
                            Console.WriteLine("🚨 ALERTE : Anomalie critique détectée !");
                            patient.StateColor = 1;
                            await _context.SaveChangesAsync();

                            string googleMapsLink = $"https://www.google.com/maps?q={latitude},{longitude}";
                            string message = $"Votre ami {nomPatient} a une hyperglycémie. " + "veuillez le contacter au plus vite" +
                                             $"Voici sa position : {googleMapsLink}";

                            await _smsService.SendSmsAsync(_emergencyPhone, message, UIDPat);
                            //FIXME:ya aucune alerte en court avant d'ajouter celle ci (state =0/1)
                            //
                            var alert = new Alert
                            {
                                AlertID = Guid.NewGuid(),
                                PatientUID = UIDPat,
                                Color = "rouge",
                                State = 0,
                                Descrip = "Cardiovascular Anomaly",
                                CreatedAt = DateTime.Now,
                                IsRead = false,
                                Location = "Inconnue",
                                latitudePatient = latitude,
                                longitudePatient = longitude
                            };

                            await _context.Alerts.AddAsync(alert);
                            await _context.SaveChangesAsync();

                            return alert;
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur pendant l'analyse : {ex.Message}");
                    Console.WriteLine(ex.StackTrace); // Ajoute ceci
                    return null;
                }

            }

        }
    }
}