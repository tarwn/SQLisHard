using SQLisHard.Domain;
using SQLisHard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SQLisHard.Controllers
{
	public class StatementController : ApiController
	{
		// POST api/<controller>
		public StatementResult Post([FromBody]Statement value)
		{
			// create command
			// execute command

			// return result
			return new StatementResult() {
				LessonId = value.LessonId,
				CompletesLesson = false,
				Content = value.Content
			};
		}

	}
}