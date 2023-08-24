using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Models;
using MeetingRoomWebApp.AutoGen;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;

namespace MeetingRoom.Controllers;

public class AdminController : Controller
{
	private readonly MeetingRoomDbContext _db;

	public AdminController(MeetingRoomDbContext db)
	{
		_db = db;
	}

	public IActionResult Index(int? userId)
	{
		userId = (int?)TempData.Peek("UserID");
		if (userId != null)
		{
			return View();
		}
		return RedirectToAction("Index", "Login");
	}

	public IActionResult Users()
	{
		List<User> users = _db.Users.Include(u => u.Roles).ToList();

		var roles = _db.Roles.Select(r => new { r.RoleId, r.RoleName }).ToList();

		var rolesJson = JsonConvert.SerializeObject(roles, Formatting.Indented);

		ViewBag.Roles = roles;

		return View(users);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Route("Admin/UpdateUser")]
	public IActionResult UpdateUser(User model)
	{
		System.Console.WriteLine("Passed Username: " + model.Username);
		System.Console.WriteLine("Passed Email: " + model.Email);
		System.Console.WriteLine("Passed Role Id: " + model.RoleId);
		System.Console.WriteLine("Passed User Id: " + model.UserId);

		var user = _db.Users?.Find(model.UserId);
		using (_db)
		{
			if (user != null)
			{
				user.RoleId = model.RoleId;
			}
			_db.SaveChanges();
		}

		return RedirectToAction("Users");
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Route("Admin/DeleteUser")]
	public IActionResult DeleteUser(long? userId)
	{
		using (_db)
		{
			var user = _db.Users?.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);

			if (user != null && user.Roles?.RoleName != null)
			{
				if (!user.Roles.RoleName.Equals("Admin"))
				{
					_db.Users?.Remove(user);
					_db.SaveChanges();
				}
			}
		}
		return RedirectToAction("Users");
	}

	public IActionResult RoomList()
	{
		List<Room> roomList = _db.Rooms.ToList();
		return View("RoomList", roomList);
	}
}
