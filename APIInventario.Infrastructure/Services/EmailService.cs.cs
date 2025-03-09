using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace APIInventario.Infrastructure.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config["MailSettings:DisplayName"], _config["MailSettings:Mail"]));
            email.To.Add(MailboxAddress.Parse(destinatario));
            email.Subject = asunto;

            var builder = new BodyBuilder { HtmlBody = mensaje };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_config["MailSettings:Host"], int.Parse(_config["MailSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_config["MailSettings:Mail"], _config["MailSettings:Password"]);

            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
