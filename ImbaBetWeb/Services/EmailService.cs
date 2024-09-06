using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using System.Net.Mail;
using System.Net;

namespace ImbaBetWeb.Services
{
    public class EmailService(string host, int port, bool enableSSL, string userName, string password) : IEmailSender
    {
        private readonly string _host = host;
        private readonly int _port = port;
        private readonly bool _enableSSL = enableSSL;
        private readonly string _userName = userName;
        private readonly string _password = password;

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = _enableSSL
            };

            return client.SendMailAsync(
                new MailMessage(_userName, email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }
}
