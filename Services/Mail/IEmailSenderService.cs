using DOT_NET_WEB_API_AUTH.Core.Mailing;

namespace DOT_NET_WEB_API_AUTH.Services.Mail
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
