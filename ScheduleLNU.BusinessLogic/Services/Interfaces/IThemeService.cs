using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.DataAccess.Entities;

namespace ScheduleLNU.BusinessLogic.Services.Interfaces
{
    public interface IThemeService
    {
        public Task<IEnumerable<ThemeDto>> GetAllAsync();

        Task AddAsync(Theme theme);

        Task EditAsync(ThemeDto theme);

        Task<Theme> GetAsync(int themeId);

        Task DeleteAsync(Theme theme);

        Task SelectAsync(Theme theme);

        Task DeselectAsync();
    }
}
