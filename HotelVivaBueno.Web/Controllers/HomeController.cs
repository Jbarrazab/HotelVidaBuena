using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HotelVivaBueno.Web.Models;
using HotelVivaBueno.Data.Data;
using HotelVivaBueno.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelVivaBueno.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var total = await _context.Employees.CountAsync();
        var active = await _context.Employees.CountAsync(e => e.Status == EmployeeStatus.Active);
        var vacation = await _context.Employees.CountAsync(e => e.Status == EmployeeStatus.OnVacation);

        var model = new DashboardViewModel
        {
            TotalEmployees = total,
            ActiveEmployees = active,
            OnVacationEmployees = vacation
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
