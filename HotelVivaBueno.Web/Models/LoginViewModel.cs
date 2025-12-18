using System.ComponentModel.DataAnnotations;

namespace HotelVivaBueno.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Document Number")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
