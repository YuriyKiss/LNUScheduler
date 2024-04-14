using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.Presentation.Controllers
{
    [Route("")]
    [Authorize]
    public class SchedulesController : Controller
    {
        private readonly IScheduleService scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            this.scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewSchedules()
        {
            IEnumerable<ScheduleDto> resList = await scheduleService.GetAllAsync();
            return View(resList);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(int scheduleId)
        {
            await scheduleService.DeleteAsync(scheduleId);
            if (ModelState.IsValid)
            {
                return StatusCode(204);
            }

            return StatusCode(400);
        }

        [HttpGet]
        [Route("delete")]
        public IActionResult DeletePopup(ScheduleDto scheduleDto)
        {
            return PartialView("_DeletePopUpPartial", scheduleDto);
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(string scheduleTitle)
        {
            await scheduleService.AddAsync(scheduleTitle);
            if (ModelState.IsValid)
            {
                return StatusCode(201);
            }

            return StatusCode(400);
        }

        [HttpGet]
        [Route("add")]
        public IActionResult AddPopup()
        {
            return PartialView("_AddPopUpPartial", new ScheduleDto());
        }

        [Route("edit")]
        public async Task<IActionResult> Edit(int scheduleId, string title)
        {
            await scheduleService.EditAsync(scheduleId, title);
            if (ModelState.IsValid)
            {
                return StatusCode(205);
            }

            return StatusCode(400);
        }

        [HttpGet]
        [Route("edit")]
        public IActionResult EditPopup(ScheduleDto scheduleDto)
        {
            return PartialView("_EditPopUpPartial", scheduleDto);
        }
    }
}
