namespace WEBAPPP.Interface
{
    
public interface ISmsService
{
    Task<(string MessageSid, string Status)> SendSmsAsync(string toNumber, string message, Guid idPatient);
}



}
