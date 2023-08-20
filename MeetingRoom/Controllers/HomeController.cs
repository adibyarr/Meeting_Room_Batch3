using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Models;
using MeetingRoomWebApp.AutoGen;

namespace MeetingRoom.Controllers;

public class HomeController : Controller
{
    private readonly MeetingRoomDbContext _db;

    public HomeController(MeetingRoomDbContext db)
    {
        _db = db;
    }

    public IActionResult Index(int? userId)
    {
        userId = (int?)TempData["UserID"];
        if (userId == null)
        {
            return RedirectToAction("Index", "Login");
        }

        using (_db)
        {
            var user = _db.Users?.Where(u => u.UserId == userId)
                                .Select(u => new { u.UserName, u.Email, u.Role, u.UserId })
                                .FirstOrDefault();

            if (user != null)
            {
                TempData["Username"] = user.UserName;
                TempData["Email"] = user.Email;
                TempData["Role"] = user.Role;
                TempData["UserID"] = user.UserId;
                return View("Home");
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
