﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SQLisHard.Controllers
{
    public class ExerciseController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Old()
		{
			return View();
		}

    }
}
