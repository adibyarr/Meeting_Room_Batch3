using System.Globalization;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace MeetingRoom.Controllers;

public class BookingController : Controller
{
	private readonly MeetingRoomDbContext _db;
	
	public BookingController(MeetingRoomDbContext db)
	{
		_db = db;
	}

	public IActionResult Index(long? userId)
	{
		// userId = Convert.ToInt32(TempData["UserID"]);
		userId = (int?)TempData.Peek("UserID");
		using (_db)
		{
			var user = _db.Users?.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);
			return View("Booking",user);
		}
	}
	
	[HttpPost]
	[Route("Booking/AvailableRooms")]
	public IActionResult AvailableRooms(long? userId, string startDate, string endDate, string startTime, string endTime, int capacity)
	{
		DateOnly.TryParse(startDate, out DateOnly parsedStartDate);
		DateOnly.TryParse(endDate, out DateOnly parsedEndDate);
		Console.WriteLine(parsedStartDate);
		Console.WriteLine(parsedEndDate);
		
		TimeOnly.TryParse(startTime, out TimeOnly parsedStartTime);
		TimeOnly.TryParse(endTime, out TimeOnly parsedEndTime);
		Console.WriteLine(parsedStartTime);
		Console.WriteLine(parsedEndTime);
		
		userId = (int?)TempData.Peek("UserID");
		Console.WriteLine(userId);
		
		using (_db)
		{
			var user = _db.Users?.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);
			return View("AvailableRooms",user);
		}
	}
	
	[Route("Booking/Book")]
	public IActionResult Book()
	{
		// booking logic
		return View("Booking");
	}
	
	
}