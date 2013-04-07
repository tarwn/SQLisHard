using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SQLisHard.Controllers
{
    public class AboutController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Exercises()
		{
			return View();
		}

		public ActionResult Tech()
		{
			return View();
		}

		public ActionResult Updates()
		{
			return View();
		}

    }
}
