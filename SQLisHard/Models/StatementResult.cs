using SQLisHard.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Models
{
	public class StatementResult : QueryResult
	{
		public string LessonId { get; set; }
		public bool CompletesLesson { get; set; }
	}
}