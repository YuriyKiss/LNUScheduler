using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;

namespace ScheduleLNU.BusinessLogic.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly ICookieService cookieService;
        private readonly UserManager<Student> userManager;

        public RegisterService(
            ICookieService cookieService,
            UserManager<Student> userManager)
        {
            this.cookieService = cookieService;
            this.userManager = userManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
        {
            var user = new Student
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                NormalizedUserName = $"{registerDto.FirstName} {registerDto.LastName}"
            };

            var registerResult = await userManager.CreateAsync(user, registerDto.Password);

            if (registerResult.Succeeded)
            {
                await cookieService.SetCookies(("studentId", user.Id));
            }

            return registerResult;
        }
    }
}
