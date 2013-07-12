using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Domain.Exercises;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLisHard.Domain.ExerciseEvaluator
{
    public class ExerciseResultEvaluator : IExerciseResultEvaluator
    {
        private IQueryEngine _queryEngine;
		private IHistoryStore _historyStore;
		private IExerciseStore _exerciseStore;

        public ExerciseResultEvaluator(IQueryEngine queryEngine, IExerciseStore exerciseStore, IHistoryStore historyStore)
        {
            _queryEngine = queryEngine;
			_exerciseStore = exerciseStore;
			_historyStore = historyStore;
        }

        public StatementResult Evaluate(Statement statement)
        {
            var queryResult = _queryEngine.ExecuteQuery(statement);

			var evaluationResult = new StatementResult(queryResult)
			{
				ExerciseId = statement.ExerciseId,
				CompletesExercise = true	// think happy thoughts
			};

			// pattern check, if defined
			var exercise = _exerciseStore.GetExercise(statement.ExerciseSetId, statement.ExerciseId);
			if (!String.IsNullOrWhiteSpace(exercise.Pattern) && evaluationResult.ExecutionStatus == QueryExecutionStatus.Success)
			{
				if (!Regex.IsMatch(statement.Content, exercise.Pattern)) {
					evaluationResult.CompletesExercise = false;
					evaluationResult.Tip = exercise.PatternTip;
				}
			}

			// evaluation of results, if still good
			if(evaluationResult.CompletesExercise)			
            {
				evaluationResult.CompletesExercise = EvaluateResultSet(statement, queryResult);
            }

			_historyStore.AddToHistory(statement.RequestorId, statement.Content, (int) evaluationResult.ExecutionStatus, evaluationResult.CompletesExercise);

			return evaluationResult;
        }

        public bool EvaluateResultSet(Statement statement, QueryResult queryResult)
        {
			if (queryResult.ExecutionStatus == QueryExecutionStatus.Error)
				return false;

			var expectedResult = _exerciseStore.GetExerciseResultForComparison(statement.ExerciseSetId, statement.ExerciseId);

			return expectedResult != null && expectedResult.Equals(queryResult);
        }
    }
}
