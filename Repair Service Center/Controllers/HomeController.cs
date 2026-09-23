using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Repair_Service_Center.Models;

namespace Repair_Service_Center.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var student = new StudentInfo()
        {
            FullName = "Didyk Maksym",
            Group = "SEs-26-1",
            ProjectTopic = "Repair Service Center"
        };
            
        return View(student);
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