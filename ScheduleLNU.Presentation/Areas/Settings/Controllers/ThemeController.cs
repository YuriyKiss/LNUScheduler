using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleLNU.BusinessLogic.Constants;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.DTOs.Mappers;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.Presentation.Areas.Settings.Controllers
{
    [Area("settings")]
    [Route("[area]/themes")]
    [Authorize]
    public class ThemeController : Controller
    {
        private readonly IThemeService themeService;

        public ThemeController(IThemeService themeService)
        {
            this.themeService = themeService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Themes()
        {
            var allThemes = await themeService.GetAllAsync();
            return View(allThemes);
        }

        [HttpGet]
        [Route("theme")]
        public IActionResult Theme(ThemeDto themeDto)
        {
            return View(themeDto);
        }

        [HttpGet]
        [Route("theme-preview")]
        public IActionResult ThemePreview()
        {
            return View(ThemeConstants.DefaultTheme);
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(ThemeDto theme)
        {
            await themeService.AddAsync(theme.ToTheme());
            return RedirectToAction(nameof(Themes));
        }

        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> Edit(ThemeDto themeDto)
        {
            await themeService.EditAsync(themeDto);
            return RedirectToAction(nameof(Themes));
        }

        [HttpGet]
        [Route("delete-confirm")]
        public IActionResult DeletePopup(ThemeDto themeDto)
        {
            return PartialView("_DeletePopUpPartial", themeDto);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(ThemeDto themeDto)
        {
            await themeService.DeleteAsync(themeDto.ToTheme());
            return RedirectToAction(nameof(Themes));
        }

        [Route("select")]
        public async Task<IActionResult> Select(ThemeDto themeDto)
        {
            await themeService.SelectAsync(themeDto.ToTheme());

            return RedirectToAction(nameof(Themes));
        }

        [Route("deselect")]
        public async Task<IActionResult> Deselect(ThemeDto themeDto)
        {
            await themeService.DeselectAsync();

            return RedirectToAction(nameof(Themes));
        }
    }
}
