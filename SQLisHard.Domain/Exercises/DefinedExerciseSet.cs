﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.Exercises
{
	public class DefinedExerciseSet
	{
		public string Title { get; set; }
		public string Summary { get; set; }
		public DefinedFinale Finale { get; set; }
		public List<DefinedExercise> Exercises { get; set; }
	}

	public class DefinedExercise
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Details { get; set; }
		public string Query { get; set; }
	}

	public class DefinedFinale
	{
		public string Title { get; set; }
		public string Details { get; set; }
	}
}