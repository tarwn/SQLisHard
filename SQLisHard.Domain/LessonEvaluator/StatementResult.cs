using SQLisHard.Domain;
using SQLisHard.Domain.DatabaseExecution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Domain.LessonEvaluator
{
	public class StatementResult : QueryResult
	{
		public string LessonId { get; set; }
		public bool CompletesLesson { get; set; }
	}
}