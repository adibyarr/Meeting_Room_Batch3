﻿using System.Diagnostics;
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
	
	public IActionResult Account(long? userId)
	{
		userId = (int?)TempData.Peek("UserID");
		User user = _db.Users.Find((long)userId);
		
		if(user != null )
		{
			return View(user);
		}
		
		return RedirectToAction("Users");
	}
	
	// [HttpGet]
	// [Route("Admin/SaveProfile")]
	// public IActionResult SaveProfile(long? userId)
	// {
	// 		var user = _db.Users.Find(userId);
	// 		if(user != null)
	// 		{
	// 			return View("NotFound");
	// 		}
	// 		var model = new User
	// 		{
	// 			UserName = user.UserName,
	// 			Email = user.Email
	// 		};
		
	// 		return View(model);
	// }
	
	[HttpPost]
	[Route("Admin/EditProfile")]
	// [ValidateAntiForgeryToken]
	// public IActionResult EditProfile(long? userId, User editedUser)
	public IActionResult EditProfile(long userId, string userName, string email, string role)
	{
		Console.WriteLine("CHECK 1");
		if(!ModelState.IsValid)
		{
			Console.WriteLine("CHECK 2");
			return View("SaveProfile");
		}
		
		User userProfile =  _db.Users.Find(userId);
		Console.WriteLine("CHECK 3");
		
		if(userProfile == null )
		{
			Console.WriteLine("CHECK 4");
			return View("NotFound");
		}
		
		userProfile.UserName = userName;
		userProfile.Email = email;
		
		Console.WriteLine($"found userID : {userProfile.UserId}");
		Console.WriteLine($"found username : {userProfile.UserName}");
		Console.WriteLine($"found email : {userProfile.Email}");
		Console.WriteLine($"found role : {userProfile.Role}");
		Console.WriteLine("---");
		Console.WriteLine($"passed userId : {userId}");
		Console.WriteLine($"passed username : {userName}");
		Console.WriteLine($"passed email : {email}");
		Console.WriteLine($"passed role : {role}");
		Console.WriteLine("CHECK 5");
		
		return RedirectToAction("Account");
	}
	
	[HttpGet]
	[Route("Admin/ProfileSaved")]
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
