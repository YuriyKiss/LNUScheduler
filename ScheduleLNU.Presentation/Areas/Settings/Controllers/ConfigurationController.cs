using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ScheduleLNU.Presentation.Controllers
{
    [Area("settings")]
    [Authorize]
    public class ConfigurationController : Controller
    {
        public ConfigurationController()
        {
        }

        [HttpGet]
        [Route("[area]")]
        public IActionResult SettingsMenu()
        {
            return View();
        }
    }
}
