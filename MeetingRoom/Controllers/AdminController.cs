using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetingRoom.Models;
using MeetingRoomWebApp.AutoGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
		List<User> users = _db.Users.ToList();

		return View(users);
		
	}
	public IActionResult Account(int? userId)
	{
		userId = (int?)TempData.Peek("UserID");
		User user = _db.Users.Find((long)userId);
		
		if(user != null )
		{
			return View(user);
		}
		return RedirectToAction("Users");
	}
	[HttpGet]
	[Route("Admin/SaveProfile")]
	public IActionResult SaveProfile(long? userId)
	
	{
			var user = _db.Users.Find(userId);
			if(user != null)
			{
				return View("NotFound");
			}
			var model = new User
			{
				UserName = user.UserName,
				Email = user.Email
			};
		
		
			return View(model);
		
	}
	[HttpPost]
	[Route("Admin/Profile")]
	[ValidateAntiForgeryToken]
	public IActionResult EditProfile(long? userId, User user)
	{
		if(!ModelState.IsValid)
		{
			return View("SaveProfile", User);
		}
		
		var userProfile =  _db.Users.Find(userId);
		if(userProfile == null )
		{
			return View("NotFound");
		}
		userProfile.UserName = user.UserName;
		userProfile.Email = user.Email;
		return RedirectToAction("ProfileSaved", User);
	}
	[HttpGet]
	public IActionResult ProfileSaved()
	{
		return View();
	}
	private bool UserExist(long? userId)
	{
		return _db.Users.Any(e => e.UserId == userId);
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
