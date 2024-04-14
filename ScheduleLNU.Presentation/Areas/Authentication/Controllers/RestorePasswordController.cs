using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.Presentation.Areas.Authentication.Controllers
{
    [Area("authentication")]
    public class RestorePasswordController : Controller
    {
        private readonly IAuthService authService;

        public RestorePasswordController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpGet]
        [Route("[area]/forgot-password")]
        public ActionResult ForgotPasswordForm()
        {
            return View();
        }

        [HttpPost]
        [Route("[area]/forgot")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            await authService.SendResetTokenAsync(forgotPasswordDto.Email);
            if (ModelState.IsValid)
            {
                return Redirect("~/authentication/login");
            }

            return new StatusCodeResult(400);
        }

        [HttpGet]
        [Route("[area]/reset-password")]
        public ActionResult ResetPasswordForm(string email, string token)
        {
            var resetPasswordDto = new ResetPasswordDto { Email = email, Token = token };
            return View(resetPasswordDto);
        }

        [HttpPost]
        [Route("[area]/reset")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var result = await authService.ResetPasswordAsync(resetPasswordDto);
            if (ModelState.IsValid && result.Succeeded)
            {
                return Redirect("~/authentication/login");
            }

            return new StatusCodeResult(400);
        }
    }
}
