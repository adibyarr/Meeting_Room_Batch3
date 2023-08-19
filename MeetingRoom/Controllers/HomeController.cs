using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Models;
using MeetingRoomWebApp.AutoGen;

namespace MeetingRoom.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        string? username = HttpContext.Session.GetString("Username");
        string? email = HttpContext.Session.GetString("Email");
        string? role = HttpContext.Session.GetString("Role");
        var userId = HttpContext.Session.GetInt32("UserId");

        using (var db = new MeetingRoomDbContext())
        {
            if (!string.IsNullOrEmpty(email))
            {
                return View();
                // if (role.Equals("Admin"))
                // {
                //     return RedirectToAction("Admin", "Home");
                // }
                // return RedirectToAction("User", "Home");
            }
        }
        return RedirectToAction("Index", "Login");
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
