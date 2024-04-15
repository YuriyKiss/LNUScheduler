using System.Threading.Tasks;

namespace ScheduleLNU.BusinessLogic.Services.EmailService
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
