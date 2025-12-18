using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HotelVivaBueno.Web.Models
{
    public class ImportViewModel
    {
        [Required]
        [Display(Name = "Excel File")]
        public IFormFile File { get; set; }
    }
}
