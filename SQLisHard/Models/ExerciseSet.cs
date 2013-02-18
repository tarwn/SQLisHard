using SQLisHard.Domain.Exercises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Models
{
	public class ExerciseSet
	{
		public string Title { get; set; }
		public string Summary { get; set; }
		public Finale Finale { get; set; }
		public List<Exercise> Exercises { get; set; }

		public ExerciseSet()
		{
			Exercises = new List<Exercise>();
		}
		public ExerciseSet(DefinedExerciseSet set)
			: this()
		{
			Title = set.Title;
			Summary = set.Summary;
			Finale = new Finale(set.Finale);
			Exercises.AddRange(set.Exercises.Select(e => new Exercise(e)));
		}
	}

}