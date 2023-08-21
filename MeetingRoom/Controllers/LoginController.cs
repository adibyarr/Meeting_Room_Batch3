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
                    var user = _db.Users?.Where(u => !string.IsNullOrEmpty(u.Email) && u.Email.Equals(email)
                                                  && !string.IsNullOrEmpty(u.Password) && u.Password.Equals(password))
                                         .FirstOrDefault();

                    if (user != null)
                    {
                        TempData["UserID"] = Convert.ToInt32(user.UserId);
                        return RedirectToAction("SaveLoginData", user.UserId);
                    }
                }
            }
            TempData["ErrorMessage"] = "Wrong Email or Password";
            return RedirectToAction("Index");
        }

        public IActionResult SaveLoginData(int? userId)
        {
            userId = (int?)TempData["UserID"];
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            using (_db)
            {
                var user = _db.Users?.Where(u => u.UserId == userId)
                                    .Select(u => new { u.Username, u.Email, u.Role, u.UserId })
                                    .FirstOrDefault();

                if (user != null)
                {
                    HttpContext.Session.SetString("UserName", user.Username);
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        HttpContext.Session.SetString("Email", user.Email);
                    }
                    if (!string.IsNullOrEmpty(user.Role))
                    {
                        HttpContext.Session.SetString("Role", user.Role);
                    }
                    HttpContext.Session.SetInt32("UserID", Convert.ToInt32(user.UserId));

                    if (!string.IsNullOrEmpty(user.Role) && user.Role.Equals("Admin"))
                    {
                        return RedirectToAction("Index", "Admin", Convert.ToInt32(user.UserId));
                    }
                    return RedirectToAction("Index", "User", Convert.ToInt32(user.UserId));
                }
            }
            return RedirectToAction("Index");
        }
    }
}
