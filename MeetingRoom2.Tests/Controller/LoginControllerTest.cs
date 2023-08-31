using FakeItEasy;
using FluentAssertions;
using MeetingRoom.Controllers;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom2.Tests;

public class LoginControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly DbContextOptions<MeetingRoomDbContext> _options;
    private readonly MeetingRoomDbContext _db;
    private readonly LoginController _controller;
    private readonly ISession _fakeSession;
    private readonly Role _admin;
    private readonly Role _manager;
    private readonly Role _engineer;
    private readonly User _adib;
    private readonly User _anjay;

    public LoginControllerTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _options = new DbContextOptionsBuilder<MeetingRoomDbContext>()
                .UseInMemoryDatabase(databaseName: "MeetingRoomReservation")
                .Options;
        _db = new MeetingRoomDbContext(_options);
        _controller = new LoginController(_db);
        _fakeSession = new FakeHttpSession();

        //Setup
        _admin = new Role() { RoleName = "Admin" };
        _manager = new Role() { RoleName = "Manager" };
        _engineer = new Role() { RoleName = "Engineer" };

        _adib = new User()
        {
            FirstName = "Adibya",
            LastName = "Rizaldy",
            Username = "adib",
            Email = "adibyarizaldy@gmail.com",
            Password = "222",
            Roles = _admin
        };

        _anjay = new User()
        {
            FirstName = "Bimajnaya",
            LastName = "Aji",
            Username = "anjay",
            Email = "anjay4705@gmail.com",
            Password = "333",
            Roles = _manager
        };

        _db.Roles.Add(_admin);
        _db.Roles.Add(_manager);
        _db.Roles.Add(_engineer);

        _db.Users.Add(_adib);
        _db.Users.Add(_anjay);

        _db.SaveChanges();

        _controller.ControllerContext.HttpContext = new DefaultHttpContext
        {
            Session = _fakeSession
        };
        _controller.ControllerContext.HttpContext.Session.SetInt32("UserID", (int)_adib.UserId);

    }

    [Fact]
    public void Return_Login_View()
    {
        // Act
        var result = _controller.Index();
        var resultView = (ViewResult)result;

        // Assert
        resultView.Should().BeOfType<ViewResult>();
        Assert.Equal("Login", resultView.ViewName);
    }

    [Fact]
    public void Redirect_Login_Prompt()
    {
        // Setup
        User anonim = new User() { Email = "anonim@gmail.com", Password = "000" };

        // Act
        var successResult = _controller.Index(_adib.Email, _adib.Password);
        var failResult = _controller.Index(anonim.Email, anonim.Password);

        var successResultRedirect = (RedirectToActionResult)successResult;
        var failRedirectResult = (RedirectToActionResult)failResult;

        // Assert
        successResultRedirect.ActionName.Should().Be("SaveLoginData");

        failRedirectResult.ActionName.Should().Be("Index");
    }

    [Fact]
    public void Redirect_SaveLoginData()
    {

        // Act
        var adminResult = _controller.SaveLoginData((int)_adib.UserId);

        _controller.ControllerContext.HttpContext.Session.SetInt32("UserID", (int)_anjay.UserId);
        var notAdminResult = _controller.SaveLoginData((int)_anjay.UserId);
        var failResult = _controller.SaveLoginData(null);

        // Assert
        var adminRedirectResult = (RedirectToActionResult)adminResult;
        var notAdminRedirectResult = (RedirectToActionResult)notAdminResult;
        var failRedirectResult = (RedirectToActionResult)failResult;

        adminRedirectResult.ActionName.Should().Be("Index");
        // adminRedirectResult?.RouteValues?["Controller"].Should().Be("Admin");
        // adminRedirectResult.ControllerName.Should().Be("Admin");

        notAdminRedirectResult.ActionName.Should().Be("Index");
        // notAdminRedirectResult.ControllerName.Should().Be("User");
        // notAdminRedirectResult?.RouteValues?["Controller"].Should().Be("User");

        failRedirectResult.ActionName.Should().Be("Index");
    }
}