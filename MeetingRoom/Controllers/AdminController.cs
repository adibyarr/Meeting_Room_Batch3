﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Models;
using MeetingRoomWebApp.AutoGen;

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
		List<User> users = _db.Users.Where(u => u.IsActive == 1).ToList();
		return View(users);
	}

	[HttpPost]
	[Route("Admin/DeleteUser/{userId:long}")]
	public IActionResult DeleteUser(long? userId)
	{
		using (_db)
		{
			var user = _db.Users?.Find(userId);

			if (user != null && !user.Role.Equals("Admin"))
			{
				user.IsActive = 0;

				_db.SaveChanges();
			}
		}
		return RedirectToAction("Users");
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

	public IActionResult RoomList()
	{
		List<Room> roomList = _db.Rooms.ToList();
		return View("RoomList", roomList);
	}
}
