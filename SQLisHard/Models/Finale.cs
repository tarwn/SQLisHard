using SQLisHard.Domain.Exercises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQLisHard.Models
{
	public class Finale
	{
		public string Title { get; set; }
		public string Details { get; set; }

		public Finale() { }

		public Finale(DefinedFinale exercise)
		{
			Title = exercise.Title;
			Details = exercise.Details;
		}
	}
}