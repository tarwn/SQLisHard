using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.Exercises
{
	public interface IExerciseStore
	{
		DefinedExerciseSet GetList(string exerciseSetId);

		DefinedExerciseResult GetExerciseResultForComparison(string exerciseId);
	}
}
