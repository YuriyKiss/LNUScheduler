using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;

namespace ScheduleLNU.BusinessLogic.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository<Schedule> scheduleRepository;

        private readonly ICookieService cookieService;

        public ScheduleService(IRepository<Schedule> scheduleRepository, ICookieService cookieService)
        {
            this.scheduleRepository = scheduleRepository;
            this.cookieService = cookieService;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync()
        {
            return (await scheduleRepository
                .SelectAllAsync(x => x.Student.Id == cookieService.GetStudentId()))
                .Select(x => new ScheduleDto { Id = x.Id, Title = x.Title })
                .OrderBy(x => x.Id);
        }

        public async Task DeleteAsync(int scheduleId)
        {
            Schedule schedule = (
                await scheduleRepository.SelectAllAsync(
                    (schedule) => schedule.Id == scheduleId && schedule.Student.Id == cookieService.GetStudentId(),
                    (entity) => entity.Student)).FirstOrDefault();
            await scheduleRepository.DeleteAsync(schedule);
        }

        public async Task AddAsync(string scheduleTitle)
        {
            await scheduleRepository.InsertAsync(
                new Schedule { Title = scheduleTitle, StudentId = cookieService.GetStudentId() });
        }

        public async Task EditAsync(int scheduleId, string scheduleTitle)
        {
            await scheduleRepository.UpdateAsync(
                new Schedule { Id = scheduleId, Title = scheduleTitle, StudentId = cookieService.GetStudentId() });
        }
    }
}
