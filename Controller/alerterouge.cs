using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using WEBAPPP.Interface;
using WEBAPPP.Services;
using LibrarySSMS;

[ApiController]
[Route("api/Start")]
public class StartAlerteSimulationTest : ControllerBase
{
    private readonly IAnomalyDetectionServiceMel _anomalyDetectionService;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly float[] _defaultTestInput = new float[]
    {
        0.7310302717932152f, 0.6612016733818098f, 0.6387595249696483f, 1.4826943775032195f,
        -1.6686628918382518f, 0.08786660898088025f, 1.1795646031459046f, 1f,
        -1.3755103460002518f, -1.416354520410437f, 0.5625246321404527f, -1.437323967157221f,
        -0.2984436891902894f, -0.9339925341498665f,
    };

    public StartAlerteSimulationTest(
        IAnomalyDetectionServiceMel anomalyDetectionService,
        AppDbContext context,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _anomalyDetectionService = anomalyDetectionService;
        _context = context;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("AlerteSimulationTest")]
    [EnableCors("AllowReactApp")]
    public async Task<IActionResult> StartAlerteSimMontre([FromForm] string IdPatientt, [FromForm] string longitude, [FromForm] string latitude)
    {
       
       
            bool startt = true;
            var resultalerte = await _anomalyDetectionService.AnalyseSmartwatchData(IdPatientt, startt, longitude, latitude);
            return Ok(resultalerte);
        

        
    }
}