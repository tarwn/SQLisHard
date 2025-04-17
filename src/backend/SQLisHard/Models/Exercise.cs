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
		public string Explanation { get; set; }
		public string Example { get; set; }
		public string ExerciseDescription { get; set; }

		public Exercise() { }

		public Exercise(DefinedExercise exercise)
		{
			Id = exercise.Id;
			Title = exercise.Title;
			Explanation = exercise.Explanation;
			Example = exercise.Example;
			ExerciseDescription = exercise.Exercise;

			//TODO remove after a few pushes as part of removing details from client
			Details = exercise.Explanation + "\n" + exercise.Example + "\n" + exercise.Exercise;
		}
	}
}