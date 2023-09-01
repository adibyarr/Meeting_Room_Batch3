using FakeItEasy;
using FluentAssertions;
using MeetingRoom.Controllers;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Tests.Controller;

public class UserControllerTests
{
    private DbContextOptions<MeetingRoomDbContext> _options;
    private MeetingRoomDbContext _db;
    private UserController? _controller;
    private HttpContext _fakeHttpContext;
    private FakeHttpSession _fakeSession;
    // private ISession _fakeSession;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<MeetingRoomDbContext>().UseInMemoryDatabase(databaseName: "MeetingRoomReservation").Options;
        _db = new MeetingRoomDbContext(_options);
        _controller = new UserController(_db);
        _fakeHttpContext = A.Fake<HttpContext>();
        _fakeSession = new FakeHttpSession();
        // _fakeSession = A.Fake<ISession>();
    }

    [Test]
    public void UserTest()
    {
        using (_db)
        {
            Role admin = new Role()
            {
                RoleId = 1,
                RoleName = "Admin"
            };

            User adib = new User()
            {
                UserId = 2,
                FirstName = "Adibya",
                LastName = "Rizaldy",
                Username = "adib",
                Email = "adibyarizaldy@gmail.com",
                Password = "222",
                Roles = admin
            };

            _db.SaveChanges();

            if (_controller != null)
            {
                _fakeSession.SetInt32("UserID", (int)adib.UserId);
                _fakeHttpContext.Session = _fakeSession;
                _controller.ControllerContext.HttpContext = _fakeHttpContext;
                // _controller.ControllerContext.HttpContext  = new DefaultHttpContext();
                // _controller.ControllerContext.HttpContext.Session.SetInt32("UserID", (int)adib.UserId);
                // _controller.ControllerContext.HttpContext.Request.Headers["UserID"] = adib.UserId.ToString();
                // _controller.ControllerContext.HttpContext.Session = _fakeSession;

                var result = _controller.Index((int?)adib.UserId);

                var viewResult = (ViewResult)result;
                var user = (User?)viewResult?.Model; 
                // RedirectToActionResult result2 = (RedirectToActionResult)_controller.Index(null);
                // Assert.That(result.ViewName, Is.EqualTo("Home"));
                // Assert.That(result2.ControllerName, Is.EqualTo("Login"));
                user.Should().NotBeNull();
            }
        }
    }
}
