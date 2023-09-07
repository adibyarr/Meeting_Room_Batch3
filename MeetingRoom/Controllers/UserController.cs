using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Models;
using MeetingRoomWebApp.AutoGen;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Controllers;

public class UserController : Controller
{
	private readonly MeetingRoomDbContext _db;

	public UserController(MeetingRoomDbContext db)
	{
		_db = db;
	}

	public IActionResult Index(int? userId)
	{
		userId = HttpContext.Session.GetInt32("UserID");
		if (userId != null)
		{
			var user = _db.Users?.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);
			return View("Home", user);
		}
		return RedirectToAction("Index", "Login");
	}

	public IActionResult Privacy()
	{
		if (HttpContext.Session.GetInt32("UserID") == null)
		{
			return RedirectToAction("Index", "Login");
		}

		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}

	public IActionResult RoomList()
	{
		if (HttpContext.Session.GetInt32("UserID") == null)
		{
			return RedirectToAction("Index", "Login");
		}

		List<Room> roomList = _db.Rooms.ToList();
		return View("RoomList", roomList);
	}
	public IActionResult Account(long? userId)
	{
		userId = HttpContext.Session.GetInt32("UserID");
		if (HttpContext.Session.GetInt32("UserID") == null)
		{
			return RedirectToAction("Index", "Login");
		}

		var user = _db.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);

		if (user != null)
		{
			return View("Account", user);
		}

		return View("Account", user);
	}
	[HttpPost]

	public IActionResult EditProfile(long userId, string userName, string firstName, string lastName)
	{
		if (ModelState.IsValid)
		{
			User userProfile = _db.Users.Find(userId);

			userProfile.Username = userName;
			userProfile.FirstName = firstName;
			userProfile.LastName = lastName;

			_db.SaveChanges();
		}

		return RedirectToAction("Account");
	}
}