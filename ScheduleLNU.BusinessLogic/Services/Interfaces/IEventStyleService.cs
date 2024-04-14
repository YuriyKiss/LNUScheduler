using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.DTOs;

namespace ScheduleLNU.BusinessLogic.Services.Interfaces
{
    public interface IEventStyleService
    {
        Task<IEnumerable<EventStyleDto>> GetAllAsync();

        Task DeleteAsync(int scheduleId);

        Task AddAsync(EventStyleDto eventStyleDto);

        Task<bool> EditAsync(EventStyleDto eventStyle);
    }
}