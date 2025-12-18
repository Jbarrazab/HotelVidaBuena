using HotelVivaBueno.Api.DTOs;
using HotelVivaBueno.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelVivaBueno.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result == null)
            {
                return BadRequest("User with this document or email already exists.");
            }
            return Ok(new { message = "Registration successful. Please check your email." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
            {
                return Unauthorized("Invalid credentials.");
            }
            return Ok(new { token });
        }
    }
}
