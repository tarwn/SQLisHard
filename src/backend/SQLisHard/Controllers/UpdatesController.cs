
using Microsoft.AspNetCore.Mvc;

[Route("updates")]
public class UpdatesController : Controller
{

    public ActionResult Index()
    {
        return View();
    }

}
