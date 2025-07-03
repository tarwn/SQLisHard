using SQLisHard.Core;
using SQLisHard.Core.Models;
using SQLisHard.Domain;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Domain.ExerciseEvaluator
{
	public class Statement : Query
	{
		public UserId RequestorId { get; set; }
		public string ExerciseId { get; set; }
		public string ExerciseSetId { get; set; }

	}
}