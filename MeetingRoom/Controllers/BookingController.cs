// using System.Globalization;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarAPI;
using Microsoft.Extensions.Configuration.UserSecrets;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

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
			return View("Booking", user);
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
		
		Console.WriteLine($"parsed DateTime start : {parsedStartDate.ToDateTime(parsedStartTime)}");
		Console.WriteLine($"parsed DateTime end : {parsedEndDate.ToDateTime(parsedEndTime)}");
		Console.WriteLine($"DateTime now : {DateTime.Now}");
		
		List<Room> roomsCap = FilterCapacity(capacity);
		foreach (var room in roomsCap)
		{
			Console.WriteLine($"{room.RoomName} : {room.Capacity}");
		}
		
		FilterDate(roomsCap, startDate, endDate, startTime, endTime);
		
		userId = (int?)TempData.Peek("UserID");
		
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

	private List<Room> FilterCapacity(int minCap)
	{
		return _db.Rooms.Where(room => room.Capacity >= minCap).ToList();
	}
	
	private void FilterDate(
		List<Room> rooms, 
		string startDate, 
		string endDate, 
		string startTime, 
		string endTime)
	{
		DateOnly.TryParse(startDate, out DateOnly parsedStartDate);
		DateOnly.TryParse(endDate, out DateOnly parsedEndDate);
		TimeOnly.TryParse(startTime, out TimeOnly parsedStartTime);
		TimeOnly.TryParse(endTime, out TimeOnly parsedEndTime);
		
		ServiceAccountCredential credential = ServiceAccount.GenerateCredential();
		CalendarService service = CalendarManager.GenerateService(credential);
		
		foreach (var room in rooms)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(service, room.Link);
			
			Events events = CalendarManager.MakeRequest(
				service,
				calendar,
				parsedStartDate.ToDateTime(parsedStartTime),
				parsedEndDate.ToDateTime(parsedEndTime)
			);
			
			List<Event> eventList = events.Items.ToList();
			
			CalendarManager.ListingEvents(eventList);
		}
	}
}