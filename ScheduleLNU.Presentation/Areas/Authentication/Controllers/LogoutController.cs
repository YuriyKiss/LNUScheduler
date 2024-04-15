using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.Presentation.Areas.Authentication.Controllers
{
    [Area("authentication")]
    [Route("[area]/logout")]
    [Authorize]
    public class LogoutController : Controller
    {
        private readonly ICookieService cookieService;

        public LogoutController(ICookieService cookieService)
        {
            this.cookieService = cookieService;
        }

        [Route("")]
        public IActionResult Logout()
        {
            cookieService.LogOut();
            return RedirectToAction("Login", "Login");
        }
    }
}
