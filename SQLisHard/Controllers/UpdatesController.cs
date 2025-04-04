
using Microsoft.AspNetCore.Mvc;

[Route("[controller]/[action]")]
public class UpdatesController : Controller
{

    public ActionResult Index()
    {
        return View();
    }

}
