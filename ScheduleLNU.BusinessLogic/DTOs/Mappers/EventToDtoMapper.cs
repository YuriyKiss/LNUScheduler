using ScheduleLNU.DataAccess.Entities;

namespace ScheduleLNU.BusinessLogic.DTOs.Mappers
{
    public static class EventToDtoMapper
    {
        public static Event ToEvent(this EventDto eventDto, EventStyle eventStyle)
        {
            return new Event()
            {
                Id = eventDto.Id,
                Title = eventDto.Title,
                StartTime = eventDto.StartTime,
                Description = eventDto.Description,
                EndTime = eventDto.EndTime,
                Links = eventDto.Links,
                Place = eventDto.Place,
                Style = eventStyle
            };
        }
    }
}
