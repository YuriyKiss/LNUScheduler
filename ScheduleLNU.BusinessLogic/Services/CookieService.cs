using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using ScheduleLNU.BusinessLogic.Extensions;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.BusinessLogic.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CookieService(IHttpContextAccessor contextAccessor)
        {
            httpContextAccessor = contextAccessor;
        }

        public Claim GetClaim(string key)
        {
            return httpContextAccessor.HttpContext.GetClaim(key);
        }

        public string GetStudentId()
        {
            return httpContextAccessor.HttpContext.GetStudentId();
        }

        public async Task LogOut()
        {
            await httpContextAccessor.HttpContext.SignOutAsync("Identity.Application");
        }

        public async Task SetCookies(params (object, object)[] claimsCookie)
        {
            await httpContextAccessor.HttpContext.SignInAsync(claimsCookie);
        }

        public void SetSessionData(params (object, object)[] data)
        {
            httpContextAccessor.HttpContext.SetSessionData(data);
        }

        public string GetSessionData(string key)
        {
            return httpContextAccessor.HttpContext.GetSessionData(key);
        }
    }
}
