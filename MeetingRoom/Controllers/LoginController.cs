using Microsoft.AspNetCore.Mvc;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Http;

namespace MeetingRoom.Controllers
{
    public class LoginController : Controller
    {
        private readonly MeetingRoomDbContext _db;

        public LoginController(MeetingRoomDbContext db)
        {
            _db = db;
        }

        // GET: Login
        public IActionResult Index()
        {
            return View("Login");
        }

        // POST: Login
        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            if (ModelState.IsValid)
            {
                using (_db)
                {
                    var user = _db.Users?.Where(u => u.Email.Equals(email) && u.Password.Equals(password))
                                         .FirstOrDefault();

                    if (user != null)
                    {
                        TempData["UserID"] = Convert.ToInt32(user.UserId);
                        return RedirectToAction("Index", "Home", user.UserId);
                    }
                }
            }
            TempData["ErrorMessage"] = "Login Attempt Failed!";
            return View("Login");
        }
    }
}
