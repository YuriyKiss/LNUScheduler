using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ScheduleLNU.BusinessLogic.DTOs;

namespace ScheduleLNU.BusinessLogic.Services.Interfaces
{
    public interface IRegisterService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto registerDto);
    }
}
