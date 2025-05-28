using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using WEBAPPP.Interface;
using System;
using System.Threading.Tasks;
using LibrarySSMS;
using LibrarySSMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;




public class SmsService : ISmsService
{

    // TWILIO INFO A METTRE ICI
    private readonly string accountSid = "AC583426ada3a6190671253cc0f59ba734";  // Remplace avec ton SID
    private readonly string authToken = "3aff7e981b5fce757d525fd39819a712";  // Remplace avec ton Auth Token
    private readonly string fromNumber = "+16087057635"; // Numéro Twilio
    private readonly AppDbContext _context;

    public SmsService( AppDbContext context)
    {
        _context = context;
        // Initialiser Twilio avec les informations d'identification
        TwilioClient.Init(accountSid, authToken);
    }
    

    public async Task<(string MessageSid, string Status)> SendSmsAsync(string toNumber, string message, Guid idpatient)
    {
        
        var messageStatus = await MessageResource.CreateAsync(
            body: message,
            from: new PhoneNumber(fromNumber),
            to: new PhoneNumber(toNumber)
        );

        return (messageStatus.Sid, "SMS envoyé");
    }
}