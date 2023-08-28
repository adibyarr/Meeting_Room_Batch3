// using System.Globalization;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarAPI;
using Microsoft.Extensions.Configuration.UserSecrets;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System.Globalization;

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
		userId = HttpContext.Session.GetInt32("UserID");
		if (userId == null)
		{
			return RedirectToAction("Index", "Login");
		}
		
		using (_db)
		{
			var user = _db.Users?.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);
			return View("Booking", user);
		}
	}

	[HttpPost]
	[Route("Booking/AvailableRooms")]
	[Obsolete]
	public IActionResult AvailableRooms(long? userId, string startDate, string endDate, string startTime, string endTime, int capacity)
	{
		Console.WriteLine($"Input Start Date: {startDate} {startTime}");
		Console.WriteLine($"Input End Date: {endDate} {endTime}");

		// DateOnly.TryParse(startDate, CultureInfo.InvariantCulture, out DateOnly parsedStartDate);
		// TimeOnly.TryParse(startTime, CultureInfo.InvariantCulture, out TimeOnly parsedStartTime);

		// DateOnly.TryParse(endDate, CultureInfo.InvariantCulture, out DateOnly parsedEndDate);
		// TimeOnly.TryParse(endTime, CultureInfo.InvariantCulture, out TimeOnly parsedEndTime);

		// DateTime.TryParse($"{parsedStartDate} {parsedStartTime}", out DateTime start);
		// DateTime.TryParse($"{parsedEndDate} {parsedEndTime}", out DateTime end);

		// Console.WriteLine($"parsed DateTime start : {start}");
		// Console.WriteLine($"parsed DateTime end : {end}");
		// Console.WriteLine($"DateTime now : {DateTime.Now}");

		List<Room> roomsCap = FilterCapacity(capacity);
		foreach (var room in roomsCap)
		{
			Console.WriteLine($"{room.RoomName} : {room.Capacity}");
		}

		FilterDate_NAR(roomsCap, startDate, endDate, startTime, endTime);

		userId = (int?)TempData.Peek("UserID");

		using (_db)
		{
			var user = _db.Users?.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);
			return View("AvailableRooms", user);
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

	[Obsolete]
	private void FilterDate_NAR(
		List<Room> rooms,
		string startDate,
		string endDate,
		string startTime,
		string endTime)
	{
		UserCredential credential = GoogleOAuth.GenerateCredential();
		CalendarService service = CalendarManager.GenerateService(credential);

		DateTime.TryParse($"{startDate} {startTime}", CultureInfo.InvariantCulture, out DateTime startInput);
		DateTime.TryParse($"{endDate} {endTime}", CultureInfo.InvariantCulture, out DateTime endInput);
		System.Console.WriteLine("Start Input : " + startInput);
		System.Console.WriteLine("End Input : " + endInput);

		TimeSpan timeDiff = TimeOnly.FromDateTime(endInput) - TimeOnly.FromDateTime(startInput);
		List<OptionRoom> optionRoomList = new List<OptionRoom>();

		foreach (var room in rooms)
		{
			if (room.Link != null)
			{
				Google.Apis.Calendar.v3.Data.Calendar calendar = CalendarManager.GenerateCalendar(service, room.Link);

				Events events = CalendarManager.MakeRequest(
					service,
					calendar,
					startInput,
					endInput
				);

				// this list contains all events ranging from datetime Start - datetime End
				// not just events that only occurs in particular time in some days.
				// conclusion : still need some filtering process
				List<Event> eventList = CalendarManager.GetEventList(events);

				// Helper variable, as first event End DateTime
				DateTime eventEnd = DateOnly.FromDateTime(startInput).ToDateTime(new TimeOnly(00, 00, 00));

				foreach (var anEvent in eventList)
				{
					if (anEvent.Start.DateTime != null && anEvent.End.DateTime != null)
					{
						DateTime eventStart = (DateTime)anEvent.Start.DateTime;
						DateOnly eventDateStart = DateOnly.FromDateTime(eventStart);

						// Helper variable, for new Time Addition
						// TimeSpan eventDiff = new TimeSpan();
						// Looping till there's an event before or 00:00
						Console.WriteLine($"Start of the Event: {eventStart}");
						DateTime eventStartStunt = eventStart;
						// Event eventTime;
						do
						{
							// var decremental = new TimeSpan(00, -30, 00);
							// eventDiff += decremental;
							eventStartStunt.AddMinutes(-30);
							Console.WriteLine($"Start of the Event on Loop: {eventStartStunt}");

							// eventTime = eventList.FirstOrDefault(e => e.End.DateTime == eventStartStunt);
						} while (eventStartStunt != eventEnd || (eventStartStunt.Hour == 0 && eventStartStunt.Minute == 0));
						eventEnd = (DateTime)anEvent.End.DateTime;
						optionRoomList.Add(new OptionRoom(room.RoomName, eventStartStunt, eventStart));
					}
				}

				// CalendarManager.ListingEvents(eventList);
			}

			foreach (var optRoom in optionRoomList)
			{
				Console.WriteLine($"{optRoom.RoomName}, {optRoom.StartDate} - {optRoom.EndDate}");
			}
		}
	}
}