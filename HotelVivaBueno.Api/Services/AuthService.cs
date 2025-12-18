using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using HotelVivaBueno.Api.DTOs;
using HotelVivaBueno.Data.Data;
using HotelVivaBueno.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HotelVivaBueno.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<Employee?> RegisterAsync(RegisterDto dto)
        {
            // Check if exists
            if (await _context.Employees.AnyAsync(e => e.DocumentNumber == dto.DocumentNumber || e.Email == dto.Email))
            {
                return null; // Already exists
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DocumentNumber = dto.DocumentNumber,
                PasswordHash = passwordHash,
                PhoneNumber = dto.PhoneNumber,
                JobTitle = dto.JobTitle,
                DepartmentId = dto.DepartmentId,
                HireDate = DateTime.UtcNow,
                Status = Data.Enums.EmployeeStatus.Active
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Send Email
            await _emailService.SendEmailAsync(employee.Email, "Bienvenido a Hotel Viva Bueno", 
                $"Hola {employee.FirstName}, tu cuenta ha sido creada exitosamente.");

            return employee;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.DocumentNumber == dto.DocumentNumber);

            if (employee == null || !BCrypt.Net.BCrypt.Verify(dto.Password, employee.PasswordHash))
            {
                return null;
            }

            // Generate JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "SuperSecretKeyForDevelopmentOnly12345!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                    new Claim(ClaimTypes.Name, employee.DocumentNumber), // Using DocumentNumber as Name
                    new Claim(ClaimTypes.Email, employee.Email),
                    new Claim("DepartmentId", employee.DepartmentId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
