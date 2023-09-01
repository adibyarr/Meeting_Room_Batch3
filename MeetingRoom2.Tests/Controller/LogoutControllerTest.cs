using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MeetingRoom.Controllers;
using MeetingRoomWebApp.AutoGen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MeetingRoom2.Tests.Controller
{
    public class LogoutControllerTest : IClassFixture<WebApplicationFactory<MeetingRoomDbContext>>
    {
        private readonly WebApplicationFactory<MeetingRoomDbContext> _factory;
        private readonly LogoutController _controller;
        private readonly ISession _fakeSession;

        public LogoutControllerTest(WebApplicationFactory<MeetingRoomDbContext> factory)
        {
            _factory = factory;
            _controller = new LogoutController();
            _fakeSession = new FakeHttpSession();

            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                Session = _fakeSession
            };
            _controller.ControllerContext.HttpContext.Session.SetInt32("UserID", 1);
        }

        [Fact]
        public void Logout_Return_Login_View()
        {
            // Act
            var result = _controller.Index();

            var resultView = (RedirectToActionResult)result;

            // Assert
            resultView.ActionName.Should().Be("Index");

        }
    }
}