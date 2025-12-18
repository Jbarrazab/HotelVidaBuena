using HotelVivaBueno.Api.DTOs;
using HotelVivaBueno.Api.Services;
using HotelVivaBueno.Data.Data;
using HotelVivaBueno.Data.Entities;
using HotelVivaBueno.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using BCrypt.Net;

namespace HotelVivaBueno.Tests.Unit
{
    public class AuthServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed departments
            _context.Departments.Add(new Department { Id = 1, Name = "Test Dept", Description = "Test" });
            _context.SaveChanges();

            // Setup configuration with JWT secret
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Secret", "TestSecretKeyForUnitTestingPurposesOnly12345!"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authService = new AuthService(_context, _configuration, null); // Email service not needed for these tests
        }

        [Fact]
        public async Task RegisterAsync_ShouldHashPassword_WhenCreatingEmployee()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                DocumentNumber = "DOC123",
                Password = "TestPassword123!",
                JobTitle = "Tester",
                DepartmentId = 1
            };

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.DocumentNumber == "DOC123");
            Assert.NotNull(employee);
            
            // Password should be hashed (not plain text)
            Assert.NotEqual("TestPassword123!", employee.PasswordHash);
            
            // Verify BCrypt hash can be validated
            Assert.True(BCrypt.Net.BCrypt.Verify("TestPassword123!", employee.PasswordHash));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnJwtToken_WhenCredentialsAreValid()
        {
            // Arrange
            var password = "ValidPassword123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var employee = new Employee
            {
                FirstName = "Test",
                LastName = "User",
                Email = "login@example.com",
                PhoneNumber = "9876543210",
                DocumentNumber = "LOGIN123",
                PasswordHash = hashedPassword,
                JobTitle = "Tester",
                Salary = 60000,
                HireDate = DateTime.UtcNow,
                Status = EmployeeStatus.Active,
                EducationLevel = EducationLevel.Bachelor,
                DepartmentId = 1
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                DocumentNumber = "LOGIN123",
                Password = "ValidPassword123!"
            };

            // Act
            var token = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.StartsWith("eyJ", token); // JWT tokens start with "eyJ"
        }
    }
}
