using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.DTOs;

namespace ScheduleLNU.BusinessLogic.Services.Interfaces
{
    public interface ILoginService
    {
        Task<bool> LogInAsync(LoginDto loginDto);
    }
}
