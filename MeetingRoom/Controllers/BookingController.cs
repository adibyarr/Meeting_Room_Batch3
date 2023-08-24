using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.Controllers;

public class BookingController : Controller
{
	private readonly MeetingRoomDbContext _db;
	
	public BookingController(MeetingRoomDbContext db)
	{
		_db = db;
	}

	public IActionResult Index()
	{
		return View("Booking");
	}
	
	public IActionResult Book()
	{
		return View("Booking");
	}
}