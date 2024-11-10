public interface IMailService
{
    Task SendEmailAsync(string to, string subject, string message, string from);
}
