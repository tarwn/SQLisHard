using NUnit.Framework;
using SQLisHard.Domain.Exercises;
using SQLisHard.Domain.Exercises.ExerciseStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SQLisHard.Tests
{
	[TestFixture]
	public class ExerciseFileVerification
	{

		[Test]
		public void EnsureExerciseFilesAreValid()
		{
			var filelist = Directory.EnumerateFiles("../../../SQLisHard/Exercises");
			var errors = new List<string>();
			foreach (var file in filelist)
			{
				try
				{
					var fileContent = File.ReadAllText(file);
					var parsedFile = FlatFileExerciseStore.ParseFile(fileContent);
				}
				catch (Exception exc)
				{
					errors.Add(String.Format("{0}: {1} - {2}", file, exc.GetType().Name, exc.Message));
				}
			}

			if (errors.Count > 0)
				Assert.Fail("Errors parsing one or more files:\n" + String.Join("\n", errors));
		}

		[Test]
		public void EnsurePatternsAreValidForSampleQueries()
		{
			var filelist = Directory.EnumerateFiles("../../../SQLisHard/Exercises");
			var errors = new List<string>();
			var sets = new List<DefinedExerciseSet>();
			foreach (var file in filelist)
			{
				try
				{
					var fileContent = File.ReadAllText(file);
					sets.Add(FlatFileExerciseStore.ParseFile(fileContent));
				}
				catch (Exception exc)
				{
					errors.Add(String.Format("{0}: {1} - {2}", file, exc.GetType().Name, exc.Message));
				}
			}

			if (errors.Count > 0)
				Assert.Fail("Errors parsing one or more files:\n" + String.Join("\n", errors));

			var exercisesWithPatterns = sets.SelectMany(s => s.Exercises.Where(e => !String.IsNullOrWhiteSpace(e.Pattern)));
			foreach (var exercise in exercisesWithPatterns)
			{
				if (!Regex.IsMatch(exercise.Query, exercise.Pattern))
					errors.Add(String.Format("Exercise {0}'s pattern does not match it's own sample query", exercise.Id));
			}

			if (errors.Count > 0)
				Assert.Fail("Errors validating exercise patterns:\n" + String.Join("\n", errors));
		}

	}
}
