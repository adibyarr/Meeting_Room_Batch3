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
    public class UserControllerTest : IClassFixture<WebApplicationFactory<MeetingRoomDbContext>>
    {
        private readonly WebApplicationFactory<MeetingRoomDbContext> _factory;
        private readonly DbContextOptions<MeetingRoomDbContext> _options;
        private readonly MeetingRoomDbContext _db;
        private readonly UserController _controller;
        private readonly ISession _fakeSession;
        private readonly Role _engineer;
        private readonly User _hamirf;
        private readonly Room _andong;
        private readonly Room _bibi;

        public UserControllerTest(WebApplicationFactory<MeetingRoomDbContext> factory)
        {
            _factory = factory;
            _options = new DbContextOptionsBuilder<MeetingRoomDbContext>()
                       .UseInMemoryDatabase(databaseName: "MeetingRoomReservation")
                       .Options;
            _db = new MeetingRoomDbContext(_options);
            _controller = new UserController(_db);
            _fakeSession = new FakeHttpSession();

            // Setup
            _andong = new Room()
            {
                // RoomId = 1,
                RoomName = "Andong",
                Capacity = 5,
                Description = "One TV",
                Link = "www.yyy.com"
            };

            _bibi = new Room()
            {
                // RoomId = 2,
                RoomName = "Bibi",
                Capacity = 17,
                Description = "One Television, One Round Table",
                Link = "www.zzz.com"
            };

            _engineer = new Role()
            {
                // RoleId = 1,
                RoleName = "Engineer"
            };

            _hamirf = new User()
            {
                // UserId = 1,
                Username = "hamirf",
                FirstName = "Haidar",
                LastName = "Amir Faruqi",
                Email = "hamirfaruqi@gmail.com",
                Password = "111",
                Roles = _engineer
            };

            _db.Rooms.Add(_andong);
            _db.Rooms.Add(_bibi);
            _db.Roles.Add(_engineer);
            _db.Users.Add(_hamirf);
            _db.SaveChanges();

            // Act
            _controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                Session = _fakeSession
            };
            _controller.ControllerContext.HttpContext.Session.SetInt32("UserID", Convert.ToInt32(_hamirf.UserId));

        }

        [Fact]
        public void Return_User_Home_View()
        {
            // Act
            var result = _controller.Index((int)_hamirf.UserId);
            var resultView = (ViewResult)result;
            var user = (User?)resultView.Model;

            // Assert
            resultView.ViewName.Should().Be("Home");
            user.Should().NotBeNull();
            user.Should().Be(_hamirf).As<User>();
        }

        [Fact]
        public void Return_Room_List_View()
        {
            // Setup
            List<Room> roomList = new List<Room>
            {
                _andong,
                _bibi
            };

            // Act
            var result = _controller.RoomList();
            var resultView = (ViewResult)result;
            var rooms = (List<Room>?)resultView.Model;

            // Assert
            resultView.ViewName.Should().Be("RoomList");

            rooms.Should().NotBeNull();
            rooms.Should().HaveCountGreaterThan(0);

        }

        [Fact]
        public void Return_Account_View()
        {
            // Act
            var result = _controller.Account(Convert.ToInt32(_hamirf.UserId));
            var resultView = (ViewResult)result;
            var user = (User?)resultView.Model;

            // Assert
            resultView.ViewName.Should().Be("Account");
            user.Should().NotBeNull();
            user.Should().Be(_hamirf).As<User>();
        }

        [Fact]
        public void Edit_Profile_Should_Back_To_Account_Action()
        {
            // Act
            _hamirf.Username = "haidaramir";
            _hamirf.FirstName = "Haidar Amir";
            _hamirf.LastName = "Faruqi";
            _db.SaveChanges();
            
            var result = _controller.EditProfile(_hamirf.UserId, _hamirf.Username, _hamirf.FirstName, _hamirf.LastName);
            var redirectResult = (RedirectToActionResult)result;

            // Assert
            redirectResult.ActionName.Should().Be("Account");
        }
    }
}