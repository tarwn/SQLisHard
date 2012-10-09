using SQLisHard.Domain;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Domain.LessonEvaluator
{
	public class StatementResult : QueryResult
	{
        public StatementResult(QueryResult queryResult) : base(queryResult) {}

		public string LessonId { get; set; }
		public bool CompletesLesson { get; set; }
	}
}