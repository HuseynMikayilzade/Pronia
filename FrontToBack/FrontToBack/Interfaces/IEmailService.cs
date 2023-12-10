namespace FrontToBack.Interfaces
{
    public interface IEmailService
    {
         Task SendEmailAsync(string ToEmail, string subject, string body, bool ishtml);
    }
}
