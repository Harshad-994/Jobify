namespace BLL.Interfaces;

public interface IEmailService
{
    void SendEmail(string emailAddress, string body, string subject);
}
