﻿using SQLisHard.Domain;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Domain.ExerciseEvaluator
{
	public class StatementResult : QueryResult
	{
        public StatementResult(QueryResult queryResult) : base(queryResult) {}

		public string ExerciseId { get; set; }
		public bool CompletesExercise { get; set; }
	}
}