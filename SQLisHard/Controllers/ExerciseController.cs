
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SQLisHard.Controllers;

[Route("[controller]/[action]")]
public class ExerciseController : Controller
{

    [Route("~/[controller]")]
    [Authorize(AuthenticationSchemes = "sih-cookie,sih-guest")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Old()
    {
        return View();
    }

}
