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
	
	public IActionResult Book()
	{
		// booking logic
		return View("Booking");
	}
}