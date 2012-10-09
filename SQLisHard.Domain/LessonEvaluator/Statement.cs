using SQLisHard.Domain;
using SQLisHard.Domain.DatabaseExecution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Domain.LessonEvaluator
{
	public class Statement : Query
	{
		public string LessonId { get; set; }
	}
}