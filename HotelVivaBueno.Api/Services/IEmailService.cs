using System.Threading.Tasks;

namespace HotelVivaBueno.Api.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
