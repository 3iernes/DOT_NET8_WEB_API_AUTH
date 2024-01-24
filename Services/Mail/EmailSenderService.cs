using DOT_NET_WEB_API_AUTH.Core.Mailing;
using MailKit.Net.Smtp;
using MimeKit;

namespace DOT_NET_WEB_API_AUTH.Services.Mail
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ILogger<EmailSenderService> _logger;
        public EmailSenderService(EmailConfiguration emailConfiguration, 
            ILogger<EmailSenderService> logger)
        {
            _emailConfiguration = emailConfiguration;
            _logger = logger;
        }
        public async Task SendEmailAsync(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendEmailAsync(emailMessage);
        }
        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Email", _emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }
        private async Task SendEmailAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client
                        .ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port,
                        MailKit.Security.SecureSocketOptions.None);
                    //client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client
                        .AuthenticateAsync(_emailConfiguration.UserName, _emailConfiguration.Password);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    _logger.LogInformation("Error SendEmailAsync");
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
