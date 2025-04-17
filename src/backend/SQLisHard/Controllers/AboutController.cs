using Microsoft.AspNetCore.Mvc;

namespace SQLisHard.Controllers;


[Route("[controller]/[action]")]
public class AboutController : Controller
{

	[Route("~/[controller]")]
	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Exercises()
	{
		return View();
	}

	public IActionResult Tech()
	{
		return View();
	}

}

