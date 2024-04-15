using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Identity;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.EmailService;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;

namespace ScheduleLNU.BusinessLogic.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Student> userManager;
        private readonly IEmailSender emailSender;

        public AuthService(UserManager<Student> userManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        public async Task SendResetTokenAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new ArgumentException($"User is not found");
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var message = new Message(
                new string[] { email },
                "LNU Schedule, Reset Password",
                "https://schedule-lnu-rg.azurewebsites.net/authentication/reset-password/?email=" + email + "&token=" + HttpUtility.UrlEncode(resetToken));
            await emailSender.SendEmailAsync(message);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmedPassword)
            {
                throw new ArgumentException($"New password should be equal to confirmed!");
            }

            return await userManager.ResetPasswordAsync(
                user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
        }
    }
}
