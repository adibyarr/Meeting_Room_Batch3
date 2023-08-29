


using CalendarAPI;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.Controllers;

public class CalendarController : Controller
{
	private readonly MeetingRoomDbContext _db;
	private readonly BookingController _bookingController;
	
	public CalendarController(MeetingRoomDbContext db, BookingController booking)
	{
		_db = db;
		_bookingController = booking;
	}
	[HttpGet]
	public IActionResult GetCalendarEvent()
	{
		UserCredential credential = GoogleOAuth.GenerateCredential();
		CalendarService service = CalendarManager.GenerateService(credential);
		List<Event> events = new List<Event>();
		
		List<Room> rooms = _db.Rooms.ToList();
		foreach(var room in rooms)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(service, room.Link);
			Console.WriteLine($"{room.Link} MASOKKK");
			DateTime startDateTime = DateTime.Now;
			DateTime endDateTime = startDateTime.AddHours(1);
			
			Events eventNam = CalendarManager.MakeRequest(service, calendar, startDateTime, endDateTime);
			
			if(eventNam.Items != null)
			{
				events.AddRange(eventNam.Items);
			}
		}
		return View();
	}
	
	
}