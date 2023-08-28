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
	[Obsolete]
	public IActionResult AvailableRooms(long? userId, string startDate, string endDate, string startTime, string endTime, int capacity)
	{
		Console.WriteLine($"Input Start Date: {startDate} {startTime}");
		Console.WriteLine($"Input End Date: {endDate} {endTime}");

		DateOnly.TryParse(startDate, CultureInfo.InvariantCulture, out DateOnly parsedStartDate);
		TimeOnly.TryParse(startTime, CultureInfo.InvariantCulture, out TimeOnly parsedStartTime);

		DateOnly.TryParse(endDate, CultureInfo.InvariantCulture, out DateOnly parsedEndDate);
		TimeOnly.TryParse(endTime, CultureInfo.InvariantCulture, out TimeOnly parsedEndTime);

		DateTime.TryParse($"{parsedStartDate} {parsedStartTime}", out DateTime start);
		DateTime.TryParse($"{parsedEndDate} {parsedEndTime}", out DateTime end);

		Console.WriteLine($"parsed DateTime start : {start}");
		Console.WriteLine($"parsed DateTime end : {end}");
		Console.WriteLine($"DateTime now : {DateTime.Now}");

		List<Room> roomsCap = FilterCapacity(capacity);
		foreach (var room in roomsCap)
		{
			Console.WriteLine($"{room.RoomName} : {room.Capacity}");
		}

		FilterDate(roomsCap, start, end);

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
	private void FilterDate(
		List<Room> rooms,
		DateTime startDate,
		DateTime endDate)
	{
		UserCredential credential = GoogleOAuth.GenerateCredential();
		CalendarService service = CalendarManager.GenerateService(credential);

		TimeSpan timeDiff = TimeOnly.FromDateTime(endDate) - TimeOnly.FromDateTime(startDate);
		List<OptionRoom> optionRoomList = new List<OptionRoom>();

		foreach (var room in rooms)
		{
			if (room.Link != null)
			{
				Google.Apis.Calendar.v3.Data.Calendar calendar = CalendarManager.GenerateCalendar(service, room.Link);

				Events events = CalendarManager.MakeRequest(
					service,
					calendar,
					startDate,
					endDate
				);

				// this list contains all events ranging from datetime Start - datetime End
				// not just events that only occurs in particular time in some days.
				// conclusion : still need some filtering process
				List<Event> eventList = CalendarManager.GetEventList(events);
				TimeOnly startTimeOnly = TimeOnly.FromDateTime(startDate);
				TimeOnly endTimeOnly = TimeOnly.FromDateTime(endDate);

				TimeOnly startEventTime = new TimeOnly();
				TimeOnly endEventTime = new TimeOnly();

				Console.WriteLine(startTimeOnly);
				Console.WriteLine(endTimeOnly);

				foreach (var anEvent in eventList)
				{
					if (anEvent.Start.DateTime != null && anEvent.End.DateTime != null)
					{
						DateOnly eventDateStart = DateOnly.FromDateTime((DateTime)anEvent.Start.DateTime);
						TimeOnly eventTimeStart = TimeOnly.FromDateTime((DateTime)anEvent.Start.DateTime);

						DateOnly eventDateEnd = DateOnly.FromDateTime((DateTime)anEvent.End.DateTime);
						TimeOnly eventTimeEnd = TimeOnly.FromDateTime((DateTime)anEvent.End.DateTime);

						endEventTime = eventTimeStart;

						TimeSpan eventDiff = new TimeSpan();
						TimeOnly startOfEvent = eventTimeStart;
						// Perulangan till there's an event before or 00:00
						Console.WriteLine($"Start of the Event: {eventTimeStart}");
						while (eventTimeEnd != startOfEvent || startOfEvent == new TimeOnly(00, 00, 00))
						{
							var decremental = new TimeSpan(00, -30, 00);
							eventDiff += decremental;
							startOfEvent.Add(decremental);
							Console.WriteLine($"Start of the Event on Loop: {startOfEvent}");

							var eventTime = eventList.FirstOrDefault(e => e.End.DateTime == eventDateStart.ToDateTime(startOfEvent));

							if (eventTime != null)
							{
								break;
							}
						}
						Console.WriteLine($"Start of the Event: {startOfEvent}");
						if (startOfEvent != eventTimeStart)
						{
							startEventTime = eventTimeStart.AddHours(eventDiff.Hours).AddMinutes(eventDiff.Minutes);
							DateTime eventStart = eventDateStart.ToDateTime(startEventTime);
							DateTime eventEnd = eventDateEnd.ToDateTime(endEventTime);
							optionRoomList.Add(new OptionRoom(room.RoomName, eventStart, eventEnd));
						}

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