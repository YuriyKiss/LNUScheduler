using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ScheduleLNU.BusinessLogic.Constants;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;

namespace ScheduleLNU.BusinessLogic.Services
{
    public class LoginService : ILoginService
    {
        private readonly ICookieService cookieService;
        private readonly IRepository<Student> studentRepository;
        private readonly UserManager<Student> userManager;

        public LoginService(
            ICookieService cookieService,
            IRepository<Student> studentRepository,
            UserManager<Student> userManager)
        {
            this.cookieService = cookieService;
            this.studentRepository = studentRepository;
            this.userManager = userManager;
        }

        public async Task<bool> LogInAsync(LoginDto loginDto)
        {
            var user = await studentRepository.SelectAsync(x => x.Email == loginDto.Email, s => s.SelectedTheme);
            var loginSuccessful = await userManager.CheckPasswordAsync(user, loginDto.Password);

            if (loginSuccessful)
            {
                await cookieService.SetCookies(("studentId", user.Id));
                if (user.SelectedTheme is null)
                {
                    cookieService.SetSessionData(
                        (ThemeConstants.FontSizeKey, ThemeConstants.DefaultTheme.FontSize),
                        (ThemeConstants.FontFamilyKey, ThemeConstants.DefaultTheme.Font),
                        (ThemeConstants.BackColorKey, ThemeConstants.DefaultTheme.BackColor),
                        (ThemeConstants.ForeColorKey, ThemeConstants.DefaultTheme.ForeColor));
                }
                else
                {
                    cookieService.SetSessionData(
                      (ThemeConstants.FontSizeKey, user.SelectedTheme.FontSize),
                      (ThemeConstants.FontFamilyKey, user.SelectedTheme.Font),
                      (ThemeConstants.BackColorKey, user.SelectedTheme.BackColor),
                      (ThemeConstants.ForeColorKey, user.SelectedTheme.ForeColor));
                }
            }

            return loginSuccessful;
        }
    }
}
