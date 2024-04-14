using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.DTOs;

namespace ScheduleLNU.BusinessLogic.Services.Interfaces
{
    public interface IScheduleService
    {
       Task<IEnumerable<ScheduleDto>> GetAllAsync();

       Task DeleteAsync(int scheduleId);

       Task AddAsync(string scheduleTitle);

       Task EditAsync(int scheduleId, string scheduleTitle);
    }
}
