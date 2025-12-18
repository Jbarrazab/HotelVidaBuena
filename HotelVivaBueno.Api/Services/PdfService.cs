using HotelVivaBueno.Data.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HotelVivaBueno.Api.Services
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            // Set license to Community as requested implies using free tools
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateCvPdf(Employee employee)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text($"Curriculum Vitae - {employee.FirstName} {employee.LastName}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text($"Cargo: {employee.JobTitle}");
                            x.Item().Text($"Departamento: {employee.Department?.Name ?? "N/A"}");
                            x.Item().Text($"Email: {employee.Email}");
                            x.Item().Text($"Teléfono: {employee.PhoneNumber}");
                            x.Item().Text($"Nivel Educativo: {employee.EducationLevel}");
                            x.Item().Text($"Perfil Profesional: {employee.ProfessionalProfile ?? "Sin información"}");
                            
                            x.Item().Text($"Salario: {employee.Salary:C}");
                            x.Item().Text($"Fecha Ingreso: {employee.HireDate.ToShortDateString()}");
                            x.Item().Text($"Estado: {employee.Status}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generado por Hotel Viva Bueno System");
                            x.Span($" - {System.DateTime.Now}");
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
