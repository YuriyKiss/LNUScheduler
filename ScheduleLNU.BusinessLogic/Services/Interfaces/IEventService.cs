using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.DTOs;

namespace ScheduleLNU.BusinessLogic.Services.Interfaces
{
    public interface IEventService
    {
        Task<(string scheduleTitle, IEnumerable<EventDto> events)> GetEventsDataAsync(int scheduleId);

        Task AddAsync(EventDto eventDto);

        Task EditAsync(EventDto eventDto);

        Task DeleteAsync(EventDto eventDto);
    }
}
