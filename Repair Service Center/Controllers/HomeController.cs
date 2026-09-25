using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repair_Service_Center.Data;
using Repair_Service_Center.Models;
using Repair_Service_Center.Models.ViewModels;

namespace Repair_Service_Center.Controllers;

public class HomeController : Controller
{
    private readonly RepairServiceContext _context;

    public HomeController(RepairServiceContext context)
    {
        _context = context;
    }

    // GET: / (home page of the service center)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Numbers for the statistics bar are passed with the ViewData dictionary
        ViewData["ServicesCount"] = await _context.PriceListItems.CountAsync();
        ViewData["TechniciansCount"] = await _context.Technicians.CountAsync();
        ViewData["ActiveOrdersCount"] = await _context.Orders
            .CountAsync(o => o.Status != OrderStatus.Issued);

        // Lists for the page are passed with a view model
        var model = new HomeIndexViewModel
        {
            DeviceTypes = await _context.DeviceTypes.OrderBy(d => d.Name).ToListAsync(),
            FeaturedServices = await _context.PriceListItems
                .Include(p => p.DeviceType)
                .Include(p => p.RepairType)
                .Include(p => p.ComplexityLevel)
                .OrderBy(p => p.Price)
                .Take(3)
                .ToListAsync()
        };

        return View(model);
    }

    // GET: /Home/About
    [HttpGet]
    public async Task<IActionResult> About()
    {
        var technicians = await _context.Technicians.OrderBy(t => t.FullName).ToListAsync();
        return View(technicians);
    }

    // GET: /Home/Author (student information, laboratory work 1)
    [HttpGet]
    public IActionResult Author()
    {
        var student = new StudentInfo
        {
            FullName = "Didyk Maksym",
            Group = "SEs-26-1",
            ProjectTopic = "Repair Service Center"
        };

        return View(student);
    }

    [HttpGet]
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
