// using System.Globalization;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarAPI;
using Microsoft.Extensions.Configuration.UserSecrets;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using NuGet.Common;
using Microsoft.AspNetCore.Http;

namespace MeetingRoom.Controllers;

public class BookingController : Controller
{
	private readonly MeetingRoomDbContext _db;
	private static CalendarService? _service;
	
	public BookingController(MeetingRoomDbContext db)
	{
		_db = db;
		InitService();
	}
	
	public void InitService()
	{
		UserCredential credential = GoogleOAuth.GenerateCredential();
		_service = CalendarManager.GenerateService(credential);
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
		userId = HttpContext.Session.GetInt32("UserID");
		if (userId == null)
		{
			return RedirectToAction("Index", "Login");
		}
		
		UserCredential credential = GoogleOAuth.GenerateCredential();
		CalendarService service = CalendarManager.GenerateService(credential);
		List<OptionRoom> optionRoomList = new();
		
		GlobalFilter(startDate, endDate, startTime, endTime, capacity);
		
		// if (startDate == null && 
		// 	endDate == null &&
		// 	startTime == null &&
		// 	endTime == null &&
		// 	capacity == 0)
		// {
		// 	//	1. All Empty
		// 	optionRoomList = Filter();
		// } 
		// else if (startDate != null &&
		// 		endDate == null &&
		// 		startTime == null &&
		// 		endTime == null &&
		// 		capacity == 0)
		// {
		// 	// 2. startDate only
		// 	optionRoomList = Filter_SD(startDate);
		// } 
		// else if (startDate == null &&
		// 		endDate != null &&
		// 		startTime == null &&
		// 		endTime == null &&
		// 		capacity == 0)
		// {
		// 	// 3. endDateonly
		// 	optionRoomList = Filter_ED(endDate);
		// } 
		// else if (startDate != null &&
		// 		endDate != null &&
		// 		startTime == null &&
		// 		endTime == null &&
		// 		capacity == 0)
		// {
		// 	// 4. startDate and endDate
		// 	Filter_SDED(startDate, endDate);
		// } 
		// else if (startDate == null &&
		// 		endDate == null &&
		// 		startTime != null &&
		// 		endTime == null &&
		// 		capacity == 0)
		// {
		// 	// 5. startTime only
		// 	Filter_ST(startTime);
		// } 
		// else if (startDate == null &&
		// 		endDate == null &&
		// 		startTime == null &&
		// 		endTime != null &&
		// 		capacity == 0)
		// {
		// 	// 6. endTime only
		// 	Filter_ET(endTime);
		// } 
		// else if (startDate == null &&
		// 		endDate == null &&
		// 		startTime != null &&
		// 		endTime != null &&
		// 		capacity == 0)
		// {
		// 	// 7. startTime and endTime
		// 	Filter_STET(startTime, endTime);
		// } 
		// else if (startDate == null &&
		// 		endDate == null &&
		// 		startTime == null &&
		// 		endTime == null &&
		// 		capacity != 0)
		// {
		// 	// 8. capacity only
		// 	Filter_Cap(capacity);
		// }
		// else if (startDate != null &&
		// 		endDate != null &&
		// 		startTime != null &&
		// 		endTime != null &&
		// 		capacity != 0)
		// {
		// 	Filter_Complete(startDate, endDate, startTime, endTime, capacity);
		// }
		
		using (_db)
		{
			var user = _db.Users?.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);
			return View("Booking", user);
		}
	}

	private List<OptionRoom> Filter()
	{
		// 1. All Empty
		
		DateTime start = DateTime.Now;
		DateTime end = DateTime.Today.AddDays(1);
		List<OptionRoom> optionRoomList = new();
		
		foreach (var room in _db.Rooms)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);
			
			List<Event> events = CalendarManager.MakeRequest(
				_service,
				calendar,
				start,
				end
			).Items.ToList();
			
			if (events.Count > 0)
			{
				DateTime? optionStart = start;
				DateTime? optionEnd = new();
				Event lastEvent = new();
				foreach (var singleEvent in events)
				{
					DateTime? singleEventStart = singleEvent.Start.DateTime;
					DateTime? singleEventEnd = singleEvent.End.DateTime;
					if (start < singleEventStart)
					{
						optionEnd = singleEventStart;
						// optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd));
						optionStart = singleEventEnd;
					} 
					else if (start > singleEventStart && start < singleEventEnd)
					{
						optionStart = singleEventEnd;
					}
					else 
					{
						optionStart = singleEventEnd;
					}
					lastEvent = singleEvent;
				}
				// optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, end));
			}
			
			if (events.Count == 0)
			{
				// optionRoomList.Add(new OptionRoom(room.RoomName, start, end));
			}
		}
		
		foreach(var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		return optionRoomList;
	}
	
	private List<OptionRoom> Filter_SD(string startDate)
	{
		// 2. startDate only
		
		DateTime start;
		DateTime end;
		DateOnly.TryParse(startDate, System.Globalization.CultureInfo.InvariantCulture, out DateOnly parsedStartDate);
		if (parsedStartDate == DateOnly.FromDateTime(DateTime.Now))
		{
			return Filter();
		} 
		else
		{
			start = new DateTime(parsedStartDate.Year, parsedStartDate.Month, parsedStartDate.Day, 0, 0, 0);
			end = start.Date.AddDays(1);
		}
		List<OptionRoom> optionRoomList = new();
		
		foreach (var room in _db.Rooms)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);
			
			List<Event> events = CalendarManager.MakeRequest(
				_service,
				calendar,
				start,
				end
			).Items.ToList();
			
			if (events.Count > 0)
			{
				DateTime? optionStart = start;
				DateTime? optionEnd = new();
				Event lastEvent = new();
				foreach (var singleEvent in events)
				{
					DateTime? singleEventStart = singleEvent.Start.DateTime;
					DateTime? singleEventEnd = singleEvent.End.DateTime;
					if (start < singleEventStart)
					{
						optionEnd = singleEventStart;
						// optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd));
						optionStart = singleEventEnd;
					} 
					else 
					{
						optionStart = singleEventEnd;
					}
					lastEvent = singleEvent;
				}
				// optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, end));
			}
			
			if (events.Count == 0)
			{
				// optionRoomList.Add(new OptionRoom(room.RoomName, start, end));
			}
		}
		
		foreach(var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		return optionRoomList;
	}

	private List<OptionRoom> Filter_ED(string endDate)
	{
		// 3. endDateonly
		
		DateTime start;
		DateTime end;
		DateOnly.TryParse(endDate, out DateOnly parsedEndDate);
		if (parsedEndDate == DateOnly.FromDateTime(DateTime.Now))
		{
			return Filter();
		} 
		else
		{
			start = DateTime.Now;
			end = new DateTime(parsedEndDate.Year, parsedEndDate.Month, parsedEndDate.Day, 0, 0, 0).AddDays(1);
		}
		List<OptionRoom> optionRoomList = new();
		
		Console.WriteLine($"initial start : {start}");
		Console.WriteLine($"initial end : {end}");

		foreach (var room in _db.Rooms)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);

			int totalDays = (end.Day - start.Day)-1;
			Console.WriteLine($"totalDays : {totalDays}");
			
			DateTime startInRoom;
			DateTime endInRoom;
			
			for (int i = 0; i <= totalDays; i++)
			{
				DateTime currentDay = start.AddDays(i);
				Console.WriteLine($"currentDay : {currentDay}");
				
				if (currentDay.Date == start.Date)
				{
					startInRoom = start;
					endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);
					Console.WriteLine("currentDay.Date == start.Date");
					Console.WriteLine($"startInRoom : {startInRoom}");
					Console.WriteLine($"endInRoom : {endInRoom}");
					Console.WriteLine("");
				} 
				else 
				{
					startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0);
					endInRoom = startInRoom.AddDays(1);
					Console.WriteLine("currentDay.Date != start.Date");
					Console.WriteLine($"startInRoom : {startInRoom}");
					Console.WriteLine($"endInRoom : {endInRoom}");
					Console.WriteLine("");
				}

				List<Event> events = CalendarManager.MakeRequest(
					_service,
					calendar,
					startInRoom,
					endInRoom
					).Items.ToList();

				if (events.Count > 0)
				{
					DateTime? optionStart = startInRoom;
					DateTime? optionEnd = new();
					Event lastEvent = new();
					foreach (var singleEvent in events)
					{
						DateTime? singleEventStart = singleEvent.Start.DateTime;
						DateTime? singleEventEnd = singleEvent.End.DateTime;
						if (startInRoom < singleEventStart)
						{
							optionEnd = singleEventStart;
							// optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd));
							optionStart = singleEventEnd;
						}
						else
						{
							optionStart = singleEventEnd;
						}
						lastEvent = singleEvent;
					}
					// optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, endInRoom));
				}

				if (events.Count == 0)
				{
					// optionRoomList.Add(new OptionRoom(room.RoomName, start, end));
				}

			}
		}

		foreach (var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		return optionRoomList;
	}

	private List<OptionRoom> Filter_SDED(string startDate, string endDate)
	{
		// 4. startDate and endDate
		
		DateTime start;
		DateTime end;
		DateOnly.TryParse(startDate, out DateOnly parsedStartDate);
		DateOnly.TryParse(endDate, out DateOnly parsedEndDate);
		
		if (parsedStartDate ==  DateOnly.FromDateTime(DateTime.Now) && parsedStartDate == parsedEndDate)
		{
			return Filter();
		}
		
		if (parsedStartDate ==  DateOnly.FromDateTime(DateTime.Now))
		{
			start = DateTime.Now;
			end = new DateTime(parsedEndDate.Year, parsedEndDate.Month, parsedEndDate.Day, 0, 0, 0).AddDays(1);
		}
		else
		{
			start = new DateTime(parsedStartDate.Year, parsedStartDate.Month, parsedStartDate.Day, 0, 0, 0);
			end = new DateTime(parsedEndDate.Year, parsedEndDate.Month, parsedEndDate.Day, 0, 0, 0).AddDays(1);
		}
		List<OptionRoom> optionRoomList = new();
		
		foreach (var room in _db.Rooms)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);
			int totalDays = (end.Day - start.Day)-1;
			
			DateTime startInRoom;
			DateTime endInRoom;
			
			for (int i = 0; i <= totalDays; i++)
			{
				DateTime currentDay = start.AddDays(i);
				
				if (currentDay.Date == start.Date)
				{
					startInRoom = start;
					endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);
				} 
				else 
				{
					startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0);
					endInRoom = startInRoom.AddDays(1);
				}
				
				List<Event> events = CalendarManager.MakeRequest(
					_service,
					calendar,
					startInRoom,
					endInRoom
					).Items.ToList();
					
				if (events.Count > 0)
				{
					DateTime? optionStart = startInRoom;
					DateTime? optionEnd = new();
					Event lastEvent = new();
					foreach (var singleEvent in events)
					{
						DateTime? singleEventStart = singleEvent.Start.DateTime;
						DateTime? singleEventEnd = singleEvent.End.DateTime;
						if (startInRoom < singleEventStart)
						{
							optionEnd = singleEventStart;
							// optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd));
							optionStart = singleEventEnd;
						}
						else
						{
							optionStart = singleEventEnd;
						}
						lastEvent = singleEvent;
					}
					// optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, endInRoom));		
				}
				
				if (events.Count == 0)
				{
					// optionRoomList.Add(new OptionRoom(room.RoomName, startInRoom, endInRoom));
				}
			}
		}
		
		foreach (var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		return optionRoomList;
	}

	private List<OptionRoom> Filter_ST(string startTime)
	{
		// 5. startTime only
		
		List<OptionRoom> optionRoomList = new();
		
		TimeOnly.TryParse(startTime, out TimeOnly parsedStartTime);
		
		DateTime start = DateOnly.FromDateTime(DateTime.Now).ToDateTime(parsedStartTime);
		DateTime end = DateTime.Today.AddDays(1);
		
		if (DateTime.Now > start)
		{
			return Filter();
		}
		
		foreach (var room in _db.Rooms)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);
			
			List<Event> events = CalendarManager.MakeRequest(
				_service,
				calendar,
				start,
				end
			).Items.ToList();
			
			if (events.Count > 0)
			{
				DateTime? optionStart = start;
				DateTime? optionEnd = new();
				Event lastEvent = new();
				foreach (var singleEvent in events)
				{
					DateTime? singleEventStart = singleEvent.Start.DateTime;
					DateTime? singleEventEnd = singleEvent.End.DateTime;
					if (start < singleEventStart)
					{
						optionEnd = singleEventStart;
						// optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd));
						optionStart = singleEventEnd;
					} 
					else if (start > singleEventStart && start < singleEventEnd)
					{
						optionStart = singleEventEnd;
					}
					else 
					{
						optionStart = singleEventEnd;
					}
					lastEvent = singleEvent;
				}
				// optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, end));
			}
			
			if (events.Count == 0)
			{
				// optionRoomList.Add(new OptionRoom(room.RoomName, start, end));
			}
		}
		
		foreach(var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		return optionRoomList;
	}

	private List<OptionRoom> Filter_ET(string endTime)
	{
		// 6. endTime only
		
		List<OptionRoom> optionRoomList = new();
		
		TimeOnly.TryParse(endTime, out TimeOnly parsedEndTime);
		
		DateTime start = DateTime.Now;
		DateTime end = DateOnly.FromDateTime(DateTime.Now).ToDateTime(parsedEndTime);
		
		if (DateTime.Now > end)
		{
			return Filter();
		}
		
		foreach (var room in _db.Rooms)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);
			
			List<Event> events = CalendarManager.MakeRequest(
				_service,
				calendar,
				start,
				end
			).Items.ToList();
			
			if (events.Count > 0)
			{
				DateTime? optionStart = start;
				DateTime? optionEnd = new();
				Event lastEvent = new();
				foreach (var singleEvent in events)
				{
					DateTime? singleEventStart = singleEvent.Start.DateTime;
					DateTime? singleEventEnd = singleEvent.End.DateTime;
					if (start < singleEventStart)
					{
						optionEnd = singleEventStart;
						// optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd));
						optionStart = singleEventEnd;
					} 
					else if (start > singleEventStart && start < singleEventEnd)
					{
						optionStart = singleEventEnd;
					}
					else 
					{
						optionStart = singleEventEnd;
					}
					lastEvent = singleEvent;
				}
				// optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, end));
			}
			
			if (events.Count == 0)
			{
				// optionRoomList.Add(new OptionRoom(room.RoomName, start, end));
			}
		}
		
		foreach(var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		return optionRoomList;
	}

	private List<OptionRoom> Filter_STET(string startTime, string endTime)
	{
		// 7. startTime and endTime
		
		List<OptionRoom> optionRoomList = new();
		
		TimeOnly.TryParse(startTime, out TimeOnly parsedStartTime);
		TimeOnly.TryParse(endTime, out TimeOnly parsedEndTime);
		
		DateTime start = DateOnly.FromDateTime(DateTime.Now).ToDateTime(parsedStartTime);
		DateTime end = DateOnly.FromDateTime(DateTime.Now).ToDateTime(parsedEndTime);
		
		if (DateTime.Now > start)
		{
			return Filter();
		}
		
		foreach (var room in _db.Rooms)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);
			
			List<Event> events = CalendarManager.MakeRequest(
				_service,
				calendar,
				start,
				end
			).Items.ToList();
			
			if (events.Count > 0)
			{
				DateTime? optionStart = start;
				DateTime? optionEnd = new();
				Event lastEvent = new();
				foreach (var singleEvent in events)
				{
					DateTime? singleEventStart = singleEvent.Start.DateTime;
					DateTime? singleEventEnd = singleEvent.End.DateTime;
					if (start < singleEventStart)
					{
						optionEnd = singleEventStart;
						// optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd));
						optionStart = singleEventEnd;
					} 
					else if (start > singleEventStart && start < singleEventEnd)
					{
						optionStart = singleEventEnd;
					}
					else 
					{
						optionStart = singleEventEnd;
					}
					lastEvent = singleEvent;
				}
				// optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, end));
			}
			
			if (events.Count == 0)
			{
				// optionRoomList.Add(new OptionRoom(room.RoomName, start, end));
			}
		}
		
		foreach(var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		return optionRoomList;
	}

	private List<OptionRoom> Filter_Cap(int capacity)
	{
		// 8. capacity only
		
		List<OptionRoom> optionRoomList = new();
		DateTime start = DateTime.Now;
		DateTime end = DateTime.Today.AddDays(1);
		
		List<Room> roomList = FilterCapacity(capacity);
		
		foreach (var room in roomList)
		{
			Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);
			
			List<Event> events = CalendarManager.MakeRequest(
				_service,
				calendar,
				start,
				end
			).Items.ToList();
			
			if (events.Count > 0)
			{
				DateTime? optionStart = start;
				DateTime? optionEnd = new();
				Event lastEvent = new();
				foreach (var singleEvent in events)
				{
					DateTime? singleEventStart = singleEvent.Start.DateTime;
					DateTime? singleEventEnd = singleEvent.End.DateTime;
					if (start < singleEventStart)
					{
						optionEnd = singleEventStart;
						optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd, capacity));
						optionStart = singleEventEnd;
					} 
					else if (start > singleEventStart && start < singleEventEnd)
					{
						optionStart = singleEventEnd;
					}
					else 
					{
						optionStart = singleEventEnd;
					}
					lastEvent = singleEvent;
				}
				optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, end, capacity));
			}
			
			if (events.Count == 0)
			{
				optionRoomList.Add(new OptionRoom(room.RoomName, start, end, capacity));
			}
		}
		
		foreach(var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		return optionRoomList;
	}

	private List<OptionRoom> Filter_Complete(
		string startDate, 
		string endDate, 
		string startTime, 
		string endTime,
		int capacity)
	{
		DateOnly.TryParse(startDate, out DateOnly parsedStartDate);
		TimeOnly.TryParse(startTime, out TimeOnly parsedStartTime);
		DateTime start = parsedStartDate.ToDateTime(parsedStartTime);
		
		DateOnly.TryParse(endDate, out DateOnly parsedEndDate);
		TimeOnly.TryParse(endTime, out TimeOnly parsedEndTime);
		DateTime end = parsedEndDate.ToDateTime(parsedEndTime);
		
		List<OptionRoom> optionRoomList = new();
		
		foreach (var room in FilterCapacity(capacity))
		{
			Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);
			
			int totalDays = (end.Day - start.Day);
			Console.WriteLine($"totalDays : {totalDays}");
			
			DateTime startInRoom;
			DateTime endInRoom;
			
			for (int i = 0; i <= totalDays; i++)
			{
				DateTime currentDay = start.AddDays(i);
				
				if (currentDay.Date == start.Date)
				{
					startInRoom = start;
					endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, end.Hour, end.Minute, end.Second);
				}
				else
				{
					startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, start.Hour, start.Minute, start.Second);
					endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, end.Hour, end.Minute, end.Second);
				}
				
				List<Event> events = CalendarManager.MakeRequest(
					_service,
					calendar,
					startInRoom,
					endInRoom
					).Items.ToList();
				
				if (events.Count == 0)
				{
					optionRoomList.Add(new OptionRoom(room.RoomName, startInRoom, endInRoom, capacity));
				}
			}
		}
		
		foreach (var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		return optionRoomList;
	}
	
	private List<Room> FilterCapacity(int minCap)
	{
		return _db.Rooms.Where(room => room.Capacity >= minCap).ToList();
	}
	
	private List<OptionRoom> GlobalFilter(string startDate, 
											string endDate,
											string startTime,
											string endTime,
											int capacity)
	{
		List<OptionRoom> optionRoomList = new();
		List<Room> roomList = new();
		DateTime start;
		DateTime end;
		
		DateOnly.TryParse(startDate, out DateOnly parsedStartDate);
		TimeOnly.TryParse(startTime, out TimeOnly parsedStartTime);
		DateOnly.TryParse(endDate, out DateOnly parsedEndDate);
		TimeOnly.TryParse(endTime, out TimeOnly parsedEndTime);
		
		// defining start
		if (startDate == null && startTime == null)
		{
			start = DateTime.Now;
		} 
		else if (startDate != null && startTime == null)
		{
			if (parsedStartDate == DateOnly.FromDateTime(DateTime.Now))
			{
				start = DateTime.Now;
			}
			else
			{
				start = new DateTime(parsedStartDate.Year, parsedStartDate.Month, parsedStartDate.Day, 0, 0, 0);	
			}
		}
		else if (startDate == null && startTime != null)
		{
			start = DateOnly.FromDateTime(DateTime.Now).ToDateTime(parsedStartTime);
		}
		else
		{
			start = parsedStartDate.ToDateTime(parsedStartTime);
		}
		
		if (start < DateTime.Now)
		{
			start = DateTime.Now;
		}
		
		// defining end
		if (endDate == null && endTime == null)
		{
			if (start.Date == DateTime.Now.Date)
			{
				end = DateTime.Today.AddDays(1);
			}
			else
			{
				end = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0).AddDays(1);
			}
		}
		else if (endDate != null && endTime == null)
		{
			end = new DateTime(parsedEndDate.Year, parsedEndDate.Month, parsedEndDate.Day, 0, 0, 0).AddDays(1);	
		}
		else if (endDate == null && endTime != null)
		{
			end = DateOnly.FromDateTime(start).ToDateTime(parsedEndTime);		
		}
		else
		{
			end = parsedEndDate.ToDateTime(parsedEndTime);
		}
		
		if (end < DateTime.Now)
		{
			end = DateTime.Today.AddDays(1);
		} // tambahi if end < start?
		
		// defining capacity --> roomList
		if (capacity == 0)
		{
			roomList = _db.Rooms.ToList();
		}
		else
		{
			roomList = FilterCapacity(capacity);
		}
		
		Console.WriteLine("------------- 1 -------------");
		Console.WriteLine($"start: {start}");
		Console.WriteLine($"end : {end}");
		Console.WriteLine($"capacity : {capacity}");

		Calendar calendar;
		UserCredential credential = GoogleOAuth.GenerateCredential();

		// main logic
		foreach (var room in roomList)
		{
			Console.WriteLine("------------- 2 -------------");
			Console.WriteLine("Entering foreach (var room in roomList)");
			_service = CalendarManager.GenerateService(credential);
			calendar = CalendarManager.GenerateCalendar(_service, room.Link);
			Console.WriteLine("Passing through GenerateCAlendar");

			int totalDays = (end.Day - start.Day);
			
			bool sameDaySpecifiedTime = false;
			if (totalDays == 0)
			{
				if (start < end)
				{
					// totalDays = 1;
					sameDaySpecifiedTime = true;
				}
			}
			if (totalDays < 0)
			{
				TimeSpan timespan = end.Date - start.Date;
				totalDays = timespan.Days;
			}
			
			DateTime startInRoom;
			DateTime endInRoom;
			
			Console.WriteLine($"totalDays : {totalDays}");
			
			DateTime lastDay;
			for (int i = 0; i <= totalDays; i++)
			{
				DateTime currentDay = start.AddDays(i);
				
				if (currentDay.Date == start.Date)
				{
					startInRoom = start;
					if (endTime != null)
					{
						endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, end.Hour, end.Minute, end.Second);	
						// Check semua kondisi yang hasilnya empty pake ini :
						if (parsedEndTime < TimeOnly.FromDateTime(DateTime.Now))
						{
							endInRoom = end;	
						}
					}
					else
					{
						endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);	
					}
					
					if (sameDaySpecifiedTime == true)
					{
						endInRoom = end;
					}
				}
				else if (currentDay.Date == end.Date)
				{
					endInRoom = end;
					if (startTime != null)
					{
						startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, start.Hour, start.Minute, start.Second);	
					}
					else
					{
						startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0);	
					}
				}
				else
				{
					if (startTime != null && endTime != null)
					{
						startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, start.Hour, start.Minute, start.Second);
						endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, end.Hour, end.Minute, end.Second);
					}
					else if (startTime != null && endTime == null)
					{
						// startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, start.Hour, start.Minute, start.Second);
						startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0);
						endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);
					}
					else if (startTime == null && endTime != null)
					{
						startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0);
						// endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, end.Hour, end.Minute, end.Second);
						endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);
					}
					else
					{
						startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0);
						endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);	
					}
				}
				
				Console.WriteLine("------------- 3 -------------");
				Console.WriteLine($"currentDat : {currentDay}");
				Console.WriteLine($"startInRoom : {startInRoom}");
				Console.WriteLine($"endInRoom : {endInRoom}");
				
				if (startInRoom == endInRoom || startInRoom > endInRoom) 
				{
					break;
				}
				
				List<Event> events = CalendarManager.MakeRequest(
					_service,
					calendar,
					startInRoom,
					endInRoom
					).Items.ToList();
					
				if (events.Count > 0)
				{
					DateTime? optionStart = startInRoom;
					DateTime? optionEnd = new();
					Event lastEvent = new();
					foreach (var singleEvent in events)
					{
						DateTime? singleEventStart = singleEvent.Start.DateTime;
						DateTime? singleEventEnd = singleEvent.End.DateTime;
						if (startInRoom < singleEventStart)
						{
							optionEnd = singleEventStart;
							optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd, room.Capacity));
							Console.WriteLine("------------- 4 -------------");
							Console.WriteLine($"Added New Option : {room.RoomName}");
							Console.WriteLine($"				 : {optionStart}");
							Console.WriteLine($"				 : {optionEnd}");	
							optionStart = singleEventEnd;
						}
						else
						{
							optionStart = singleEventEnd;
						}
						lastEvent = singleEvent;
					}
					optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, endInRoom, room.Capacity));
					Console.WriteLine("------------- 5 -------------");
					Console.WriteLine($"Added New Option : {room.RoomName}");
					Console.WriteLine($"				 : {lastEvent.End.DateTime}");
					Console.WriteLine($"				 : {endInRoom}");	
				}

				if (events.Count == 0)
				{
					optionRoomList.Add(new OptionRoom(room.RoomName, startInRoom, endInRoom, room.Capacity));
					Console.WriteLine("------------- 6 -------------");
					Console.WriteLine($"Added New Option : {room.RoomName}");
					Console.WriteLine($"				 : {startInRoom}");
					Console.WriteLine($"				 : {endInRoom}");		
				}
				lastDay = currentDay;
			}
			Console.WriteLine(" ");
			Console.WriteLine("######################");
			Console.WriteLine(" ");
		}
		
		foreach (var room in optionRoomList)
		{
			Console.WriteLine("-----");
			Console.WriteLine($"RoomName : {room.RoomName}");
			Console.WriteLine($"Start : {room.StartDate}");
			Console.WriteLine($"End : {room.EndDate}");
		}
		
		Console.WriteLine("");
		
		return optionRoomList;
	}
}