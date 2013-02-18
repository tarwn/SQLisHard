using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Domain.Exercises;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                CompletesExercise = EvaluateResultSet(statement, queryResult),
                ExerciseId = statement.ExerciseId
            };

			_historyStore.AddToHistory(statement.RequestorId, statement.Content, (int) evaluationResult.ExecutionStatus, evaluationResult.CompletesExercise);

			return evaluationResult;
        }

        public bool EvaluateResultSet(Statement statement, QueryResult queryResult)
        {
			if (queryResult.ExecutionStatus == QueryExecutionStatus.Error)
				return false;

			var expectedResult = _exerciseStore.GetExerciseResultForComparison(statement.ExerciseId);

			return expectedResult.Equals(queryResult);
        }
    }
}
