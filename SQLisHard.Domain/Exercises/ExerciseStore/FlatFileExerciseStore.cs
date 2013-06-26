using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLisHard.Domain.Exercises.ExerciseStore
{
	public class FlatFileExerciseStore : IExerciseStore
	{
		private ConcurrentDictionary<string, DefinedExerciseSet> _exercises;
		private IQueryEngine _engine;

		public FlatFileExerciseStore(IQueryEngine engine)
		{
			_exercises = new ConcurrentDictionary<string, DefinedExerciseSet>();
			_engine = engine;
		}

		public DefinedExerciseSet GetList(string exerciseSetId)
		{
			if (_exercises.ContainsKey(exerciseSetId))
				return _exercises[exerciseSetId];
			else
				throw new ArgumentOutOfRangeException("Specified exercise set could not be found");
		}

		public DefinedExerciseResult GetExerciseResultForComparison(string exerciseSetId, string exerciseId)
		{
			if (!_exercises.ContainsKey(exerciseSetId))
				return null;

			var exercise = _exercises[exerciseSetId].Exercises.Where(e => e.Id == exerciseId).FirstOrDefault();
			if (exercise == null)
				return null;

			var query = new Query() {
				Content = exercise.Query,
				LimitResults = true
			};

			var result = _engine.ExecuteQuery(query);
			return new DefinedExerciseResult(result);
		}

		public void Add(DefinedExerciseSet sampleSet)
		{
			_exercises.AddOrUpdate(sampleSet.Id, sampleSet, (key, value) => sampleSet);
		}

		public void Add(string fileContent)
		{
			var set = ParseFile(fileContent);
			_exercises.AddOrUpdate(set.Id, set, (key, value) => set);
		}

		public static DefinedExerciseSet ParseFile(string data)
		{
			var cleanData = data.Replace("\r", "");

			var linePattern = "([A-Za-z]+):[ \\t]*((?:[^\\n]|\\n\\t)+)";
			var regex = new Regex(linePattern);
			var matches = regex.Matches(cleanData);

			if (matches.Count == 0)
				throw new ArgumentException("Did not find any matching lines in the provided file");

			if (!matches[0].Groups[1].Value.Equals(FileTokens.SET_ID))
				throw new DefinedExerciseFileFormatException(String.Format("The first matched line started with '{0}' instead of Id", matches[0].Groups[1].Value));

			var result = new DefinedExerciseSet(matches[0].Groups[2].Value);
			var nextStep = ParseState.SetTitle;
			DefinedExercise lastExercise = null;

			for (int i = 1; i < matches.Count; i++)
			{
				var currentField = matches[i];
				switch (nextStep)
				{
					case ParseState.SetTitle:
						VerifyFieldIsCorrect(currentField, FileTokens.SET_TITLE);
						result.Title = currentField.Groups[2].Value;
						nextStep++;
						break;
					case ParseState.SetSummary:
						VerifyFieldIsCorrect(currentField, FileTokens.SET_SUMMARY);
						result.Summary = currentField.Groups[2].Value.Replace("\t", " ");
						nextStep++;
						break;
					case ParseState.FinaleTitle:
						VerifyFieldIsCorrect(currentField, FileTokens.FINALE_TITLE);
						result.Finale = new DefinedFinale() { Title = currentField.Groups[2].Value };
						nextStep++;
						break;
					case ParseState.FinaleDetails:
						VerifyFieldIsCorrect(currentField, FileTokens.FINALE_DETAILS);
						result.Finale.Details = currentField.Groups[2].Value.Replace("\t", " ");
						nextStep++;
						break;
					case ParseState.NextExerciseOrDone:
						VerifyFieldIsCorrect(currentField, FileTokens.EXERCISE_ID);
						lastExercise = new DefinedExercise(currentField.Groups[2].Value);
						result.Exercises.Add(lastExercise);
						nextStep++;
						break;
					case ParseState.ExerciseTitle:
						VerifyFieldIsCorrect(currentField, FileTokens.EXERCISE_TITLE);
						lastExercise.Title = currentField.Groups[2].Value;
						nextStep++;
						break;
					case ParseState.ExerciseQuery:
						VerifyFieldIsCorrect(currentField, FileTokens.EXERCISE_QUERY);
						lastExercise.Query = currentField.Groups[2].Value;
						nextStep++;
						break;
					case ParseState.ExerciseExplanation:
						VerifyFieldIsCorrect(currentField, FileTokens.EXERCISE_EXPLANATION);
						lastExercise.Explanation = currentField.Groups[2].Value.Replace("\t", " ");
						nextStep++;
						break;
					case ParseState.ExerciseExample:
					case ParseState.ExerciseExercise:
						var token = VerifyFieldMatchesOneOf(currentField, new string[] { FileTokens.EXERCISE_EXERCISE, FileTokens.EXERCISE_EXAMPLE });

						if (token.Equals(FileTokens.EXERCISE_EXAMPLE))
						{
							lastExercise.Example = currentField.Groups[2].Value.Replace("\t", " ");
							nextStep++;
						}
						else
						{
							lastExercise.Exercise = currentField.Groups[2].Value.Replace("\t", " ");
							nextStep = ParseState.NextExerciseOrDone;
						}
						break;
				}
			}

			if (nextStep != ParseState.NextExerciseOrDone)
				throw new DefinedExerciseFileFormatException(String.Format("File ended earlier then expected, currently expecting {0}", nextStep));

			return result;
		}

		private static string VerifyFieldMatchesOneOf(Match match, string[] possibleTokens)
		{
			if (match.Groups.Count != 3 || !possibleTokens.Contains(match.Groups[1].Value))
				throw new DefinedExerciseFileFormatException(String.Format("Expected one of: '{0}', but instead found '{1}'", string.Join("','", possibleTokens), match.Groups[1].Value));
			else
				return match.Groups[1].Value;
		}

		private static void VerifyFieldIsCorrect(Match match, string fieldToken)
		{
			if (match.Groups.Count != 3 || !match.Groups[1].Value.Equals(fieldToken))
				throw new DefinedExerciseFileFormatException(String.Format("Expected '{0}', but instead found '{1}'", fieldToken, match.Groups[1].Value));
		}

		private enum ParseState
		{
			SetTitle = 1,
			SetSummary = 2,
			FinaleTitle = 3,
			FinaleDetails = 4,
			NextExerciseOrDone = 5,
			ExerciseTitle = 6,
			ExerciseQuery = 7,
			ExerciseExplanation = 8,
			ExerciseExample = 9,
			ExerciseExercise = 10
		}

		private static class FileTokens
		{
			public const string SET_ID = "Id";
			public const string SET_TITLE = "Title";
			public const string SET_SUMMARY = "Summary";
			public const string FINALE_TITLE = "FinaleTitle";
			public const string FINALE_DETAILS = "FinaleDetails";
			public const string EXERCISE_ID = "ExerciseId";
			public const string EXERCISE_TITLE = "Title";
			public const string EXERCISE_QUERY = "Query";
			public const string EXERCISE_EXPLANATION = "Explanation";
			public const string EXERCISE_EXAMPLE = "Example";
			public const string EXERCISE_EXERCISE = "Exercise";

			public const string EXERCISE_DETAILS = "Details";
		}
	}
}
