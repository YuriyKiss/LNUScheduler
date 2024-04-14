using System.Collections.Generic;
using System.Linq;
using MimeKit;

namespace ScheduleLNU.BusinessLogic.Services.EmailService
{
    public class Message
    {
        public Message(IEnumerable<string> to, string subject, string body)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress(x.Split('@')[0], x)));
            Subject = subject;
            Body = body;
        }

        public Message(string from, string subject, string content)
        {
            From = new List<MailboxAddress>
            {
                new MailboxAddress(from.Split("@")[0], from)
            };
            Subject = subject;
            Body = content;
        }

        public List<MailboxAddress> To { get; set; }

        public List<MailboxAddress> From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
