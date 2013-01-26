using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SQLisHard.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Signout()
		{
			FormsAuthentication.SignOut();

			return View("Index");
		}
    }
}
