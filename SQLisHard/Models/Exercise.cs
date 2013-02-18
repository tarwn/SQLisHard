using SQLisHard.Domain.Exercises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Models
{
	public class Exercise
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Details { get; set; }

		public Exercise() { }

		public Exercise(DefinedExercise exercise)
		{
			Id = exercise.Id;
			Title = exercise.Title;
			Details = exercise.Details;
		}
	}
}