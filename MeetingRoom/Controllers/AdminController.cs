using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Models;
using MeetingRoomWebApp.AutoGen;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;
using Google.Apis.Calendar.v3.Data;
using CalendarAPI;
using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2;

namespace MeetingRoom.Controllers;

public class AdminController : Controller
{
	private readonly MeetingRoomDbContext _db;
	private static CalendarService? _service;

	public AdminController(MeetingRoomDbContext db)
	{
		_db = db;
		InitService();
	}

	public void InitService()
	{
		UserCredential credential = GoogleOAuth.GenerateCredential();
		_service = CalendarManager.GenerateService(credential);
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

	public IActionResult Users()
	{
		if (HttpContext.Session.GetInt32("UserID") == null)
		{
			return RedirectToAction("Index", "Login");
		}

		List<User> users = _db.Users.Include(u => u.Roles).ToList();

		var roles = _db.Roles.Select(r => new { r.RoleId, r.RoleName }).ToList();

		var rolesJson = JsonConvert.SerializeObject(roles, Formatting.Indented);

		ViewBag.Roles = roles;

		return View(users);
	}

	public JsonResult GetRoles()
	{
		var roles = _db.Roles.Select(r => new { r.RoleId, r.RoleName });

		return Json(roles);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Route("Admin/UpdateUser")]
	public IActionResult UpdateUser(User model)
	{
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
	[Route("Admin/DeleteUser/{userId:long}")]
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
		if (HttpContext.Session.GetInt32("UserID") == null)
		{
			return RedirectToAction("Index", "Login");
		}

		List<Room> roomList = _db.Rooms.ToList();
		return View("RoomList", roomList);
	}

	/*
	TODO:
	[ ] add warning feature if room name already exist
	[x] add edit room detail feature	
	 */

	[HttpPost]
	[Route("Admin/CreateRoom")]
	public IActionResult CreateRoom(string roomName, int capacity, string description)
	{
		if (ModelState.IsValid)
		{
			Room room = new Room
			{
				RoomName = roomName,
				Capacity = capacity,
				Description = description
			};

			var roomExisted = _db.Rooms.Where(room => room.RoomName == roomName).ToList();

			if ((roomExisted is null) || !roomExisted.Any())
			{
				Calendar calendar = new Calendar
				{
					Summary = roomName,
					TimeZone = "Asia/Jakarta",
					Description = description
				};

				Calendar newCalendar = CalendarManager.CreateCalendar(_service, calendar);
				// return RedirectToAction("RoomList");
				room.Link = newCalendar.Id;
				_db.Rooms.Add(room);
				_db.SaveChanges();
			}
		}
		return RedirectToAction("RoomList");
	}

	[HttpPost]
	[Route("Admin/DeleteRoom")]
	public IActionResult DeleteRoom(long roomId)
	{
		if (ModelState.IsValid)
		{
			var room = _db.Rooms.FirstOrDefault(room => room.RoomId == roomId);

			Console.WriteLine($"DELETE 1. Room ID : {roomId}");
			if (room is null)
			{
				Console.WriteLine($"DELETE 2. Room ID : {roomId}");
				return RedirectToAction("RoomList");
			}

			Console.WriteLine($"DELETE 3. Room ID : {roomId}");
			CalendarManager.DeleteCalendar(_service, room.Link);
			_db.Rooms.RemoveRange(room);
			_db.SaveChanges();
		}

		return RedirectToAction("RoomList");
	}

	[HttpPost]
	[Route("Admin/EditRoom")]
	public IActionResult EditRoom(long roomId, string roomName, long capacity, string description)
	{
		if (ModelState.IsValid)
		{	
			Room? room = _db.Rooms?.Find(roomId);
			
			var roomExisted = _db.Rooms.FirstOrDefault(room => room.RoomName == roomName);

			if (roomExisted == null || roomExisted.RoomId == roomId)
			{
				Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);
				calendar.Summary = roomName;
				calendar.Description = description;
				string updatedCalendar = CalendarManager.UpdateCalendar(_service, calendar, room.Link);
				// Console.WriteLine(updatedCalendar);

				room.RoomName = roomName;
				room.Capacity = capacity;
				room.Description = description;
				_db.SaveChanges();
			}
		}

		return RedirectToAction("RoomList");
	}
	
	public IActionResult Account(long? userId)
	{
		userId = HttpContext.Session.GetInt32("UserID");
		if (HttpContext.Session.GetInt32("UserID") == null)
		{
			return RedirectToAction("Index", "Login");
		}

		Console.WriteLine("--- INSIDE ACCOUNT ---");

		var user = _db.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);

		// Console.WriteLine($"from _db UserName : {user.Username}");
		// Console.WriteLine($"from _db Email : {user.Email}");
		// Console.WriteLine($"from _db Role : {user.Roles}");

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
			Console.WriteLine("--- INSIDE EDIT PROFILE ---");

			Console.WriteLine($"passed UserId : {userId}");
			Console.WriteLine($"passed UserName : {userName}");
			// Console.WriteLine($"passed Email : {email}");
			// Console.WriteLine($"passed role : {role}");

			User userProfile = _db.Users.Find(userId);

			Console.WriteLine($"from _db UserId : {userProfile.UserId}");
			Console.WriteLine($"from _db UserName : {userProfile.Username}");


			userProfile.Username = userName;
			userProfile.FirstName = firstName;
			userProfile.LastName = lastName;

			_db.SaveChanges();
		}

		return RedirectToAction("Account");
	}
}