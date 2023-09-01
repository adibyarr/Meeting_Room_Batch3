using FakeItEasy;
using MeetingRoom.Controllers;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Tests.Controller;

[TestFixture]
public class LogoutControllerTest
{
    private LogoutController? _controller;
    private ISession _fakeSession;

    [SetUp]
    public void Setup()
    {
        _controller = new LogoutController();
        _fakeSession = A.Fake<ISession>();
    }

    [Test]
    public void LogoutTest()
    {
        if (_controller != null)
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Response.Headers["UserID"] = "1";
            _controller.ControllerContext.HttpContext.Session = _fakeSession;

            RedirectToActionResult result = (RedirectToActionResult)_controller.Index();
            Assert.That(result.ControllerName, Is.EqualTo("Login"));
        }
    }
}
