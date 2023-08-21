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
		List<User> users = _db.Users.ToList();
		return View(users);
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
	
	/*
	TODO:
	- add warning feature if room name already exist
	- add edit room detail feature
	 */
	
	[HttpPost]
	public IActionResult RoomList(string roomName, int capacity, string description)
	{
		if (ModelState.IsValid)
		{
			Room room = new Room
			{
				RoomName = roomName,
				Capacity = capacity,
				Description = description
			};
			
			// Room? roomExisted = _db.Rooms.FirstOrDefault(r => r.RoomName == roomName);
			var roomExisted = _db.Rooms.Where(room => room.RoomName == roomName).ToList();
			
			if ((roomExisted is null) || !roomExisted.Any())
			{
				// return RedirectToAction("RoomList");
				_db.Rooms.Add(room);
				_db.SaveChanges();
			}

			// _db.Rooms.Add(room);
			// _db.SaveChanges();
		}
		return RedirectToAction("RoomList");
	}
	
	[HttpGet]
	public IActionResult DeleteRoom(long roomId)
	{
		if (ModelState.IsValid)
		{
			var room = _db.Rooms.Where(room => room.RoomId == roomId).ToList();

			if ((room is null) || (!room.Any()))
			{
				return RedirectToAction("RoomList");
			}

			_db.Rooms.RemoveRange(room);
			_db.SaveChanges();
		}
	
		return RedirectToAction("RoomList");
	}
}