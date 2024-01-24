using MimeKit;

namespace DOT_NET_WEB_API_AUTH.Core.Mailing
{
    public class EmailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public EmailMessage(IEnumerable<string> to, string subject, string content)
        {
            To = [.. to.Select(x => new MailboxAddress("No responder", x))];
            Subject = subject;
            Content = content;
        }
    }
}
