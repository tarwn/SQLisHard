using SQLisHard.Domain.DatabaseExecution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.LessonEvaluator
{
    public class LessonResultEvaluator : ILessonResultEvaluator
    {
        private IQueryEngine _queryEngine;

        public LessonResultEvaluator(IQueryEngine queryEngine)
        {
            _queryEngine = queryEngine;
        }

        public StatementResult Evaluate(Statement statement)
        {
            var queryResult = _queryEngine.ExecuteQuery(statement);
            return new StatementResult()
            {
                Content = queryResult.Content,
                CompletesLesson = true,
                LessonId = statement.LessonId
            };
        }
    }
}
