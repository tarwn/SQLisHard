using Microsoft.AspNetCore.Mvc;

namespace SQLisHard.Controllers;

[Route("/")]
public class HomeController : Controller
{

    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

    [HttpGet("/signout")]
    public ActionResult Signout()
    {
        SignOut();
        return View("Index");
    }

    [HttpGet("/exception")]
    public ActionResult Exception()
    {
        throw new Exception("MVC Exception Message");
    }
}

