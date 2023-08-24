using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Models;
using MeetingRoomWebApp.AutoGen;

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
			return View();
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
	
	public IActionResult RoomList()
	{
		List<Room> roomList = _db.Rooms.ToList();
		return View("RoomList",roomList);
	}
	public IActionResult Account(long? userId)
	{
		Console.WriteLine("--- INSIDE ACCOUNT ---");
		
		userId = (int?)TempData.Peek("UserID");
		User? user = _db.Users.Find((long)userId);
		
		Console.WriteLine($"from _db UserName : {user.Username}");
		Console.WriteLine($"from _db Email : {user.Email}");
		Console.WriteLine($"from _db Role : {user.Role}");
		
		if (user != null)
		{
			return View("Account",user);
		}
		
		return View("Account",user);
	}
	
	public IActionResult EditProfile(long userId, string userName, string email, string role)
	{
		if (ModelState.IsValid)
		{
			Console.WriteLine("--- INSIDE EDIT PROFILE ---");

			Console.WriteLine($"passed UserId : {userId}");
			Console.WriteLine($"passed UserName : {userName}");
			Console.WriteLine($"passed Email : {email}");
			Console.WriteLine($"passed role : {role}");
			
			User userProfile = _db.Users.Find(userId);
			
			Console.WriteLine($"from _db UserId : {userProfile.UserId}");
			Console.WriteLine($"from _db UserName : {userProfile.Username}");
			Console.WriteLine($"from _db Email : {userProfile.Email}");
			Console.WriteLine($"from _db role : {userProfile.Role}");
			
			userProfile.Username = userName;
			userProfile.Email = email;
			userProfile.Role = role;

			_db.SaveChanges();
		}
		
		return RedirectToAction("Account");
	}
}
