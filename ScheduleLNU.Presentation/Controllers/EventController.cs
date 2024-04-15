using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Extensions;
using ScheduleLNU.BusinessLogic.Services.Interfaces;

namespace ScheduleLNU.Presentation.Controllers
{
    [Authorize]
    [Route("schedule")]
    public class EventController : Controller
    {
        private readonly IEventService eventService;
        private readonly IEventStyleService eventStyleService;

        public EventController(
            IEventService eventService,
            IEventStyleService eventStyleService)
        {
            this.eventService = eventService;
            this.eventStyleService = eventStyleService;
        }

        [HttpGet]
        [Route("{scheduleId}")]
        public async Task<IActionResult> Events(int scheduleId)
        {
            var (scheduleTitle, events) = await eventService.GetEventsDataAsync(scheduleId);
            ViewBag.Title = scheduleTitle;
            ViewBag.ScheduleId = scheduleId;
            return View(events);
        }

        [HttpGet]
        [Route("add-event/{scheduleId}")]
        public async Task<IActionResult> AddEventPreview(int scheduleId)
        {
            var styles = await eventStyleService.GetAllAsync();
            var now = DateTime.Now.RemoveSeconds();
            ViewBag.EventStyles = styles;
            ViewBag.HasEventStyles = styles.Any();
            return View(new EventDto()
            {
                Title = "New event",
                ScheduleId = scheduleId,
                StartTime = now,
                EndTime = now.AddMinutes(30)
            });
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddEvent(EventDto eventDto)
        {
            await eventService.AddAsync(eventDto);
            return RedirectToAction(nameof(Events), new { eventDto.ScheduleId });
        }

        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> EditEventPreview(EventDto eventDto)
        {
            var styles = await eventStyleService.GetAllAsync();
            ViewBag.EventStyles = styles;
            ViewBag.HasEventStyles = styles.Any();
            return View(nameof(EditEventPreview), eventDto);
        }

        [HttpPost]
        [Route("edit-event")]
        public async Task<IActionResult> EditEvent(EventDto eventDto)
        {
            await eventService.EditAsync(eventDto);
            return RedirectToAction(nameof(Events), new { eventDto.ScheduleId });
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteEvent(EventDto eventDto)
        {
            await eventService.DeleteAsync(eventDto);
            return RedirectToAction(nameof(Events), new { eventDto.ScheduleId });
        }

        [HttpGet]
        [Route("delete")]
        public IActionResult DeletePopup(EventDto eventDto)
        {
            return PartialView("_DeletePopUpPartial", eventDto);
        }
    }
}
