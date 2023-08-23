using System.Diagnostics;
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
				// return RedirectToAction("RoomList");
				_db.Rooms.Add(room);
				_db.SaveChanges();
				return RedirectToAction("RoomList");
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
			var room = _db.Rooms.Where(room => room.RoomId == roomId).ToList();
			
			Console.WriteLine($"DELETE 1. Room ID : {roomId}");	
			if ((room is null) || (!room.Any()))
			{
				Console.WriteLine($"DELETE 2. Room ID : {roomId}");
				return RedirectToAction("RoomList");
			}

			Console.WriteLine($"DELETE 3. Room ID : {roomId}");
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
			
			room.RoomName = roomName;
			room.Capacity = capacity;
			room.Description = description;
			
			_db.SaveChanges();
		}
	
		return RedirectToAction("RoomList");
	}
}