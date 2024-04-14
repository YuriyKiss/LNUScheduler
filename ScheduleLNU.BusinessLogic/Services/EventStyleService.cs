using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;

namespace ScheduleLNU.BusinessLogic.Services
{
    public class EventStyleService : IEventStyleService
    {
        private readonly IRepository<EventStyle> eventStyleRepository;

        private readonly ICookieService cookieService;

        public EventStyleService(
            IRepository<EventStyle> eventStyleRepository,
            ICookieService cookieService)
        {
            this.eventStyleRepository = eventStyleRepository;
            this.cookieService = cookieService;
        }

        public async Task AddAsync(EventStyleDto eventStyleDto)
        {
            await eventStyleRepository.InsertAsync(
                new EventStyle
                {
                    Title = eventStyleDto.Title,
                    ForeColor = eventStyleDto.ForeColor,
                    BackColor = eventStyleDto.BackColor,
                    StudentId = cookieService.GetStudentId()
                });
        }

        public async Task DeleteAsync(int eventId)
        {
            var eventStyle = await eventStyleRepository.SelectAsync(
                s => s.Id == eventId &&
                s.StudentId == cookieService.GetStudentId());
            await eventStyleRepository.DeleteAsync(eventStyle);
        }

        public async Task<bool> EditAsync(EventStyleDto eventStyle)
        {
            await eventStyleRepository.UpdateAsync(
                new EventStyle
                {
                    Id = eventStyle.Id,
                    Title = eventStyle.Title,
                    ForeColor = eventStyle.ForeColor,
                    BackColor = eventStyle.BackColor,
                    StudentId = cookieService.GetStudentId()
                });
            return true;
        }

        public async Task<IEnumerable<EventStyleDto>> GetAllAsync()
        {
            return (await eventStyleRepository
                .SelectAllAsync(x => x.StudentId == cookieService.GetStudentId()))
                .Select(x => new EventStyleDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    BackColor = x.BackColor,
                    ForeColor = x.ForeColor
                })
                .OrderBy(x => x.Id);
        }
    }
}
