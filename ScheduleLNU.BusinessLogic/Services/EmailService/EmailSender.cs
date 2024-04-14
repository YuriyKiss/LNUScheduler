using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace ScheduleLNU.BusinessLogic.Services.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig emailConfig;

        public EmailSender(EmailConfig emailConfig)
        {
            this.emailConfig = emailConfig;
        }

        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            var client = new SmtpClient();
            await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, true);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);
            await client.SendAsync(mailMessage);
            await client.DisconnectAsync(true);
            client.Dispose();
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();

            if (message.To is null)
            {
                emailMessage.To.Add(new MailboxAddress(emailConfig.To.Split('@')[0], emailConfig.To));
                emailMessage.From.AddRange(message.From);
            }
            else
            {
                emailMessage.To.AddRange(message.To);
                emailMessage.From.Add(new MailboxAddress(emailConfig.From.Split('@')[0], emailConfig.From));
            }

            emailMessage.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<p>{0}</p>", message.Body) };

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }
    }
}
