


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
}