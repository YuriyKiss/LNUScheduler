using ScheduleLNU.DataAccess.Entities;

namespace ScheduleLNU.BusinessLogic.DTOs.Mappers
{
    public static class EventStyleToDtoMapper
    {
        public static EventStyle ToEventStyle(this EventStyleDto eventStyleDto)
        {
            return new EventStyle()
            {
                Id = eventStyleDto.Id,
                Title = eventStyleDto.Title,
                BackColor = eventStyleDto.BackColor,
                ForeColor = eventStyleDto.ForeColor
            };
        }
    }
}
