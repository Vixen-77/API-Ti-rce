using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using LibrarySSMS; 
using DotNetEnv; 
using LibrarySSMS.Models;
using LibrarySSMS.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Extensions;
using Swashbuckle.AspNetCore.Filters;
using WEBAPPP.Services;
using Microsoft.Extensions.Caching.Memory;
using WEBAPPP.Interface;


var builder = WebApplication.CreateBuilder(args);

// Charger les variables depuis .env
/*Env.Load();
var appUrl = Env.GetString("APIprincipal_URL"); 
var appUrl2 = Env.GetString("APIsecondaire_URL");

if (string.IsNullOrEmpty(appUrl) || string.IsNullOrEmpty(appUrl2))
{
    throw new Exception("Les variables d'environnement APP_URL et REACT_APP_URL doivent être définies dans .env");
}

// Utiliser les URLs définies dans .env
builder.WebHost.UseUrls(appUrl);*/



//ingnoré le warning du chiffrement 
//builder.Configuration.AddEnvironmentVariables();// Maintenant, il peut utiliser IEmailService

Env.Load("C:\\Users\\HP\\Documents\\L3\\PFE\\API3\\API-Ti-rce\\.env");
var appUrl = Env.GetString("API3_URL"); 
var reactAppUrl = Env.GetString("REACT_URL");


builder.WebHost.UseUrls(appUrl);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<SmartwatchNewGenService>();
builder.Services.AddScoped<SmartwatchService>();
builder.Services.AddScoped<IAnomalyDetectionServiceMel,AnomalyDetectionServiceMel>();
builder.Services.AddScoped<ISmsService,SmsService>();
builder.Services.AddScoped<CGMService>();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Logging.AddConsole();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://192.168.1.102:8081")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


         var app = builder.Build(); // ICI on fige le service !

       /*  var scope = app.Services.CreateScope();
         var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Appliquer les migrations automatiquement
         context.Database.Migrate();
           var newPro = new ProS
        {
            UID = Guid.NewGuid(),
            FullName = "John Doe", //1
            Email = "test@example.com", //2
            PasswordHash = "simple", //3
            Salt = "salttest",
            Role = RoleManager.ProfSanté, //4
            IsActive = true,
            City = "Paris", //5
            PostalCode = "75000", //6
            DateOfBirth = new DateTime(1990, 5, 15), //7
            PhoneNumber = "+33612345678", //8
            CreatedAt = DateTime.UtcNow,
            AccountStatus = false,
            SubscriptionPlan = true,
            IsAvailable = true,
            AcceptRequest = true,
            CheckedSchedule = true,
            TwoFactorEnabled = false,
            IsOnline = false,
            LastLogin = DateTime.UtcNow
        };
        Console.WriteLine("Insertion en cours...");
        context.ProSs.Add(newPro);
        context.SaveChanges();
        Console.WriteLine("Insertion réussie !");*/

// Middleware après builder.Build()
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseCors("AllowReactApp");

app.UseAuthentication(); // Toujours avant Authorization
app.UseAuthorization();
app.MapControllers(); // API classiques
app.MapGet("/", () => "Hello, ASP.NET Core APITierce! Répond parfaitement!");


app.Run();














































// **Ajout de données après app.Build()**
/*using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Appliquer les migrations automatiquement
    context.Database.Migrate();

    // Vérifier si des données existent déjà
    if (!context.TestEntities.Any()) 
    {
        context.TestEntities.Add(new TestEntity
        {
            Name = "John Doe",
            Etat = UserState.Conducteur,
            Username = "johndoe",
            Email = "johndoe@example.com",
            PasswordHash = "hashedpassword",
            Salt = "randomsalt",
            FullName = "John Doe",
            City = "Sample City",
            PostalCode = "12345",
            PhoneNumber = "123-456-7890",
            AccountStatus = AccountStatus.Active,
            Device = Device.SmartCarOBU,
            SubscriptionPlan = true,
            TwoFactorEnabled = false
        });

        context.SaveChanges();
    }
}*/