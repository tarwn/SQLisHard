using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.Exercises.ExerciseStore
{
	public class DefinedExerciseFileFormatException : Exception
	{

		public DefinedExerciseFileFormatException(string message) : base(message)
		{ }

	}
}
