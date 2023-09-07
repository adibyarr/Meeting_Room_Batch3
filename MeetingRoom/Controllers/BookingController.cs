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
using System.Globalization;
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
			return View("Booking");
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

		optionRoomList = GlobalFilter(startDate, endDate, startTime, endTime, capacity);

		using (_db)
		{
			var user = _db.Users?.Include(u => u.Roles).FirstOrDefault(u => u.UserId == userId);
			return View("Booking", optionRoomList);
		}
	}

	private List<Room> FilterCapacity(int minCap)
	{
		return _db.Rooms.Where(room => room.Capacity >= minCap).ToList();
	}

	[Obsolete]
	private List<OptionRoom> GlobalFilter(string startDate,
											string endDate,
											string startTime,
											string endTime,
											int capacity)
	{
		List<OptionRoom> optionRoomList = new();
		List<Room> roomList;
		DateTime start;
		DateTime end;

		DateOnly.TryParse(startDate, CultureInfo.InvariantCulture, out DateOnly parsedStartDate);
		TimeOnly.TryParse(startTime, CultureInfo.InvariantCulture, out TimeOnly parsedStartTime);
		DateOnly.TryParse(endDate, CultureInfo.InvariantCulture, out DateOnly parsedEndDate);
		TimeOnly.TryParse(endTime, CultureInfo.InvariantCulture, out TimeOnly parsedEndTime);

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
		}

		// defining capacity --> roomList
		if (capacity == 0)
		{
			roomList = _db.Rooms.ToList();
		}
		else
		{
			roomList = FilterCapacity(capacity);
		}

		Google.Apis.Calendar.v3.Data.Calendar calendar;
		UserCredential credential = GoogleOAuth.GenerateCredential();

		// main logic
		foreach (var room in roomList)
		{
			_service = CalendarManager.GenerateService(credential);
			calendar = CalendarManager.GenerateCalendar(_service, room.Link);

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
						if (start.Date == end.Date)
						{
							if (parsedEndTime < TimeOnly.FromDateTime(DateTime.Now))
							{
								endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);
							}
							else if (parsedEndTime > TimeOnly.FromDateTime(DateTime.Now))
							{
								endInRoom = end;
							}
						} 
						else
						{
							if (parsedEndTime < TimeOnly.FromDateTime(DateTime.Now))
							{
								endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);
								if (startDate != null && endDate != null)
								{
									endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, end.Hour, end.Minute, end.Second);		
								}
							}
							else
							{
								endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, end.Hour, end.Minute, end.Second);	
							}
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
						startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, start.Hour, start.Minute, start.Second);
						endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);
					}
					else if (startTime == null && endTime != null)
					{
						startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0);
						endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, end.Hour, end.Minute, end.Second);
					}
					else
					{
						startInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0);
						endInRoom = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 0, 0, 0).AddDays(1);
					}
				}

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
							if (optionStart != optionEnd && optionStart < optionEnd)
							{
								optionRoomList.Add(new OptionRoom(room.RoomName, optionStart, optionEnd, room.Capacity));	
							}
							optionStart = singleEventEnd;
						}
						else
						{
							optionStart = singleEventEnd;
						}
						lastEvent = singleEvent;
					}
					if (lastEvent.End.DateTime != endInRoom && lastEvent.End.DateTime < endInRoom)
					{
						optionRoomList.Add(new OptionRoom(room.RoomName, lastEvent.End.DateTime, endInRoom, room.Capacity));
					}
				}

				if (events.Count == 0)
				{
					if (startInRoom != endInRoom && startInRoom < endInRoom)
					{
						optionRoomList.Add(new OptionRoom(room.RoomName, startInRoom, endInRoom, room.Capacity));	
					}
				}
				lastDay = currentDay;
			}
		}

		return optionRoomList;
	}

	[HttpPost]
	[Route("Booking/InsertMeeting")]
	public IActionResult InsertMeeting(string summary, string description, string attendee,
									   string startDate, string endDate,
									   string meetingStartTime, string meetingEndTime,
									   string roomName, long? roomCap)
	{
		int? userId = HttpContext.Session.GetInt32("UserID");
		if (userId == null)
		{
			return RedirectToAction("Index", "Login");
		}

		using (_db)
		{
			// Get meeting creator
			var user = _db.Users?.FirstOrDefault(u => u.UserId == userId);
			if (user != null)
			{
				var creatorEmail = user.Email;
				List<EventAttendee> attenders = new List<EventAttendee>
				{
					new EventAttendee() { Email = creatorEmail }
				};
				string[] attendees = attendee.Split(",");
				for (int i = 0; i < attendees.Length; i++)
				{
					attenders.Add(new EventAttendee()
					{
						Email = attendees[i].Trim()
					});
				}

				string createdBy = $"Created by: {creatorEmail}\n\n";
				description = createdBy + description;

				// Create meeting prompt
				var room = _db.Rooms?.FirstOrDefault(r => r.RoomName.Equals(roomName));
				if (_service != null && room != null)
				{
					Google.Apis.Calendar.v3.Data.Calendar calendar = CalendarManager.GenerateCalendar(_service, room.Link);

					// Start meeting DateTime
					DateOnly.TryParse(startDate, out var startDateOnly);
					TimeOnly.TryParse(meetingStartTime, CultureInfo.InvariantCulture, out var startTimeOnly);
					DateTime start = startDateOnly.ToDateTime(startTimeOnly);

					DateOnly.TryParse(endDate, out var endDateOnly);
					TimeOnly.TryParse(meetingEndTime, CultureInfo.InvariantCulture, out var endTimeOnly);

					if (endTimeOnly != TimeOnly.MinValue)
					{
						endDateOnly = startDateOnly;
					}
					DateTime end = endDateOnly.ToDateTime(endTimeOnly);

					EventDateTime meetingStart = new EventDateTime() { DateTimeDateTimeOffset = start, TimeZone = "Asia/Jakarta" };
					EventDateTime meetingEnd = new EventDateTime() { DateTimeDateTimeOffset = end, TimeZone = "Asia/Jakarta" };

					bool isMeetingCreated = CalendarManager.CreateEvent(_service,
												calendar,
												summary,
												description,
												meetingStart,
												meetingEnd,
												attenders);
				}
			}
		}
		return View("Booking");
	}
}