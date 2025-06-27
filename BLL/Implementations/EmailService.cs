using System.Net;
using System.Net.Mail;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BLL.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string fromEmail;
    private readonly string smtpHost;
    private readonly int smtpPort;
    private readonly string username;
    private readonly string password;


    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        fromEmail = _configuration["EmailConfiguration:Email"]!;
        smtpHost = _configuration["EmailConfiguration:Host"]!;
        smtpPort = int.Parse(_configuration["EmailConfiguration:Port"]!);
        username = _configuration["EmailConfiguration:Username"]!;
        password = _configuration["EmailConfiguration:Password"]!;

    }
    public void SendEmail(string emailAddress, string body, string subject)
    {
        try
        {
            using MailMessage mm = new(fromEmail, emailAddress);
            mm.Subject = subject;
            mm.Body = body;
            mm.IsBodyHtml = true;

            SmtpClient smtp = new()
            {
                Host = smtpHost,
                EnableSsl = true,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(username, password),
                Port = smtpPort
            };

            smtp.Send(mm);

        }
        catch (Exception)
        {
            throw;
        }
    }
}
