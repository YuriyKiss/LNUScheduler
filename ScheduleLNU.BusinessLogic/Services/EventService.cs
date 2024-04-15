using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.Constants;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.DTOs.Mappers;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;

namespace ScheduleLNU.BusinessLogic.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Schedule> scheduleRepository;
        private readonly IRepository<Event> eventRepository;
        private readonly IRepository<EventStyle> eventStyleRepository;

        public EventService(
            IRepository<Schedule> scheduleRepository,
            IRepository<Event> eventRepository,
            IRepository<EventStyle> eventStyleRepository)
        {
            this.scheduleRepository = scheduleRepository;
            this.eventRepository = eventRepository;
            this.eventStyleRepository = eventStyleRepository;
        }

        public async Task<(string scheduleTitle, IEnumerable<EventDto> events)> GetEventsDataAsync(int scheduleId)
        {
            var scheduleRecord = await scheduleRepository.SelectAsync(s => s.Id == scheduleId, s => s.Events);
            var eventRecords = await eventRepository.SelectAllAsync(e => scheduleRecord.Events.Contains(e), e => e.Style);
            var events = eventRecords
                .Select(e =>
                {
                    var style = e.Style ?? ThemeConstants.DefaultEventStyle.ToEventStyle();
                    return new EventDto()
                    {
                        Id = e.Id,
                        Title = e.Title,
                        ScheduleId = scheduleId,
                        Description = e.Description,
                        EndTime = e.EndTime,
                        StartTime = e.StartTime,
                        Place = e.Place,
                        Style = style,
                        StyleId = style.Id
                    };
                }) ?? Array.Empty<EventDto>();

            return (scheduleRecord?.Title, events);
        }

        public async Task AddAsync(EventDto eventDto)
        {
            var scheduleRecord = await scheduleRepository.SelectAsync(s => s.Id == eventDto.ScheduleId, s => s.Events);
            var eventStyle = await eventStyleRepository.SelectAsync(e => e.Id == eventDto.StyleId);
            scheduleRecord.Events.Add(eventDto.ToEvent(eventStyle));
            await scheduleRepository.UpdateAsync(scheduleRecord);
        }

        public async Task EditAsync(EventDto eventDto)
        {
            var eventStyle = await eventStyleRepository.SelectAsync(e => e.Id == eventDto.StyleId);
            var eventRecord = eventDto.ToEvent(eventStyle);
            await eventRepository.UpdateAsync(eventRecord);
        }

        public async Task DeleteAsync(EventDto eventDto)
        {
            var eventStyle = await eventStyleRepository.SelectAsync(e => e.Id == eventDto.StyleId);
            await eventRepository.DeleteAsync(eventDto.ToEvent(eventStyle));
        }
    }
}
