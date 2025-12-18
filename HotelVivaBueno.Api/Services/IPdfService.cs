using HotelVivaBueno.Data.Entities;

namespace HotelVivaBueno.Api.Services
{
    public interface IPdfService
    {
        byte[] GenerateCvPdf(Employee employee);
    }
}
