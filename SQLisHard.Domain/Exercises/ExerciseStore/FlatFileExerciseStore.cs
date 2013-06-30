using SQLisHard.Domain.Exercises.ExerciseStore.Parser;
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

		public DefinedExercise GetExercise(string exerciseSetId, string exerciseId)
		{
			if (!_exercises.ContainsKey(exerciseSetId))
				return null;

			var exercise = _exercises[exerciseSetId].Exercises.Where(e => e.Id == exerciseId).FirstOrDefault();
			return exercise;
		}

		public DefinedExerciseResult GetExerciseResultForComparison(string exerciseSetId, string exerciseId)
		{
			var exercise = GetExercise(exerciseSetId, exerciseId);
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
			var transitions = FlatFileParser.DefineTransitions()
								  .InitialTransition(ParseState.SetId)
								  .AddTransition(ParseState.SetId, ParseState.SetTitle)
								  .AddTransition(ParseState.SetTitle, ParseState.SetSummary)
								  .AddTransition(ParseState.SetSummary, ParseState.FinaleTitle)
								  .AddTransition(ParseState.FinaleTitle, ParseState.FinaleDetails)
								  .AddTransition(ParseState.FinaleDetails, ParseState.NewExercise)
								  .AddTransition(ParseState.NewExercise, ParseState.ExerciseTitle)
								  .AddTransition(ParseState.ExerciseTitle, ParseState.ExerciseQuery)
								  .AddTransition(ParseState.ExerciseQuery, ParseState.ExerciseExplanation)
								  .AddTransition(ParseState.ExerciseExplanation, ParseState.ExerciseExample,
																				 ParseState.ExerciseExercise)
								  .AddTransition(ParseState.ExerciseExample, ParseState.ExerciseExercise)
								  .AddTransition(ParseState.ExerciseExercise, ParseState.NewExercise)
								  .AddFinalTransition(ParseState.ExerciseExercise);

			var cleanData = data.Replace("\r", "");
			var parser = new FlatFileParser(transitions);

			DefinedExerciseSet set = null;
			DefinedExercise currentExercise = null;

			var results = parser.ParseContent(cleanData);

			foreach (var entry in results)
			{
				var cleanValue = entry.Value.Replace("\t", " ");
				switch (entry.Config) { 
					case ParseState.SetId:
						set = new DefinedExerciseSet(cleanValue);
						break;
					case ParseState.SetTitle:
						set.Title = cleanValue;
						break;
					case ParseState.SetSummary:
						set.Summary = cleanValue;
						break;
					case ParseState.FinaleTitle:
						set.Finale = new DefinedFinale();
						set.Finale.Title = cleanValue;
						break;
					case ParseState.FinaleDetails:
						set.Finale.Details = cleanValue;
						break;
					case ParseState.NewExercise:
						currentExercise = new DefinedExercise(cleanValue);
						set.Exercises.Add(currentExercise);
						break;
					case ParseState.ExerciseTitle:
						currentExercise.Title = cleanValue;
						break;
					case ParseState.ExerciseQuery:
						currentExercise.Query = cleanValue;
						break;
					case ParseState.ExerciseExplanation:
						currentExercise.Explanation = cleanValue;
						break;
					case ParseState.ExerciseExercise:
						currentExercise.Exercise = cleanValue;
						break;
					case ParseState.ExerciseExample:
						currentExercise.Example = cleanValue;
						break;
					default:
						throw new DefinedExerciseFileFormatException(String.Format("An unexpected configuration was encountered: {0}", entry.Config));
				}
			}

			return set;
		}
	}
}
