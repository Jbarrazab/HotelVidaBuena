using System.Threading.Tasks;
using HotelVivaBueno.Api.DTOs;
using HotelVivaBueno.Data.Entities;

namespace HotelVivaBueno.Api.Services
{
    public interface IAuthService
    {
        Task<Employee?> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
