using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoom.Controllers;

public class HelloworldController : Controller
{
	// 
	// GET: /HelloWorld/
	public string Index()
	{
		return "This is my default action...";
	}
	// 
	// GET: /HelloWorld/Welcome/ 
	public IActionResult Welcome(string name, int numTimes = 2)
	{
		ViewData["Message"] = "Merbabu " + name;
		ViewData["NumTimes"] = numTimes;
		return View();
	}
}
