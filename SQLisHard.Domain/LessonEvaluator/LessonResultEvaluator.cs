using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.LessonEvaluator
{
    public class LessonResultEvaluator : ILessonResultEvaluator
    {
        private IQueryEngine _queryEngine;
		private IHistoryStore _historyStore;

        public LessonResultEvaluator(IQueryEngine queryEngine, IHistoryStore historyStore)
        {
            _queryEngine = queryEngine;
			_historyStore = historyStore;
        }

        public StatementResult Evaluate(Statement statement)
        {
            var queryResult = _queryEngine.ExecuteQuery(statement);
			var evaluationResult = new StatementResult(queryResult)
            {
                CompletesLesson = EvaluateResultSet(statement, queryResult),
                LessonId = statement.LessonId
            };

			_historyStore.AddToHistory(statement.RequestorId, statement.Content, (int) evaluationResult.ExecutionStatus, evaluationResult.CompletesLesson);

			return evaluationResult;
        }

        public bool EvaluateResultSet(Statement statement, QueryResult queryResult)
        {
            if (queryResult.ExecutionStatus == QueryExecutionStatus.Error)
                return false;

            return true;
        }
    }
}
