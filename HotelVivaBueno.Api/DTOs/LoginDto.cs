using System.ComponentModel.DataAnnotations;

namespace HotelVivaBueno.Api.DTOs
{
    public class LoginDto
    {
        [Required]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
