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
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            if (ModelState.IsValid)
            {
                using (var db = new MeetingRoomDbContext())
                {
                    var user = db.Users?.Where(u => u.Email.Equals(email) && u.Password.Equals(password))
                                        .FirstOrDefault();

                    if (user != null)
                    {
                        // HttpContext.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
                        HttpContext.Session.SetString("Username", user.UserName);
                        HttpContext.Session.SetString("Email", user.Email);
                        HttpContext.Session.SetString("Role", user.Role);
                        HttpContext.Session.SetInt32("UserId", (int)user.UserId);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }
    }
}
