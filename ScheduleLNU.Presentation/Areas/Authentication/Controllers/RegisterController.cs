using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.Presentation.Areas.Authentication.Controllers
{
    [Area("authentication")]
    [Route("[area]/register")]
    public class RegisterController : Controller
    {
        private readonly IRegisterService registerManager;

        public RegisterController(IRegisterService registerManager)
        {
            this.registerManager = registerManager;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var registerResult = await registerManager.RegisterAsync(registerDto);

                if (registerResult.Succeeded)
                {
                    return Redirect("~/");
                }

                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(registerDto);
        }
    }
}
