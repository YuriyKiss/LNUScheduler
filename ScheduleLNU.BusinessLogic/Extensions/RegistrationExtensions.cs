using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using ScheduleLNU.BusinessLogic.Constants;

namespace ScheduleLNU.BusinessLogic.Extensions
{
    public static class RegistrationExtensions
    {
        public static async Task SignInAsync(this HttpContext context, params (object key, object value)[] claimsCookies)
        {
            var claimsIdentity = new ClaimsIdentity("Identity.Application");
            claimsIdentity.AddClaims(claimsCookies.Select(cookie => new Claim(cookie.key.ToString(), cookie.value.ToString())));

            await context.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity));
        }

        public static Claim GetClaim(this HttpContext context, string keyValue)
        {
            return context.User.Claims.FirstOrDefault(c => c.Type == keyValue);
        }

        public static string GetStudentId(this HttpContext context)
        {
            var studentEmailAdressClaim = context.GetClaim("studentId");
            return studentEmailAdressClaim?.Value;
        }

        public static void SetSessionData(this HttpContext context, params (object key, object value)[] data)
        {
            foreach (var (key, value) in data)
            {
                context.Session.SetString(key.ToString(), value.ToString());
            }
        }

        public static string GetSessionData(this HttpContext context, string key)
        {
            return context.Session.GetString(key);
        }

        public static string BackColor(this HttpContext context)
        {
            return context.GetSessionData(ThemeConstants.BackColorKey);
        }

        public static string ForeColor(this HttpContext context)
        {
            return context.GetSessionData(ThemeConstants.ForeColorKey);
        }

        public static string FontFamily(this HttpContext context)
        {
            return context.GetSessionData(ThemeConstants.FontFamilyKey);
        }

        public static string FontSize(this HttpContext context)
        {
            return context.GetSessionData(ThemeConstants.FontSizeKey);
        }
    }
}
