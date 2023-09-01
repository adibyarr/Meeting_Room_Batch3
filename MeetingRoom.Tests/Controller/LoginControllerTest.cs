using FakeItEasy;
using FluentAssertions;
using MeetingRoom.Controllers;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Tests;

[TestFixture]
public class LoginControllerTests
{
    private DbContextOptions<MeetingRoomDbContext> _options;
    private MeetingRoomDbContext _db;
    private LoginController? _controller;
    // private HttpContext _fakeHttpContext;
    // private FakeHttpSession _fakeSession;
    private ISession _fakeSession;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<MeetingRoomDbContext>().UseInMemoryDatabase(databaseName: "MeetingRoomReservation").Options;
        _db = new MeetingRoomDbContext(_options);
        _controller = new LoginController(_db);
        // _fakeHttpContext = A.Fake<HttpContext>();
        // _fakeSession = new FakeHttpSession();
        _fakeSession = A.Fake<ISession>();
    }

    [Test]
    public void LoginTest()
    {
        if (_controller != null)
        {
            ViewResult result = (ViewResult)_controller.Index();
            Assert.That(result.ViewName, Is.EqualTo("Login"));
            Assert.That(result.ViewName, Is.Not.EqualTo("Index"));
        }
    }

    [Test]
    public void LoginPromptTest()
    {
        using (_db)
        {
            Role admin = new Role()
            {
                RoleId = 1,
                RoleName = "Admin"
            };

            Role manager = new Role()
            {
                RoleId = 2,
                RoleName = "Manager"
            };

            Role engineer = new Role()
            {
                RoleId = 3,
                RoleName = "Engineer"
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

            User anonymous = new User() { UserId = 99, Email = "anonymous@gmail.com", Password = "111" };

            _db.Roles.Add(admin);
            _db.Roles.Add(manager);
            _db.Roles.Add(engineer);

            _db.Users.Add(adib);

            _db.SaveChanges();

            if (_controller != null)
            {
                // _fakeSession.SetInt32("UserID", (int)adib.UserId);
                // _fakeHttpContext.Session = _fakeSession;
                // _controller.ControllerContext.HttpContext = _fakeHttpContext;
                _controller.ControllerContext.HttpContext  = new DefaultHttpContext();
                _controller.ControllerContext.HttpContext.Request.Headers["UserID"] = adib.UserId.ToString();
                _controller.ControllerContext.HttpContext.Session = _fakeSession;

                var successResult = _controller.Index(adib.Email, adib.Password);
                var failResult = _controller.Index(anonymous.Email, anonymous.Password);

                // Assert
                var successRedirectResult = (RedirectToActionResult)successResult;
                var failRedirectResult = (RedirectToActionResult)failResult;

                Assert.That(successRedirectResult.ActionName, Is.EqualTo("SaveLoginData"));
                Assert.That(failRedirectResult.ActionName, Is.EqualTo("Index"));
            }
        }
    }
}