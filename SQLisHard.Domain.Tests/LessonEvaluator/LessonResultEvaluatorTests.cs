using Moq;
using NUnit.Framework;
using SQLisHard.Domain.LessonEvaluator;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain.Tests.LessonEvaluator
{
    [TestFixture]
    public class LessonResultEvaluatorTests
    {

        [Test]
        public void Evaluate_Statement_ReturnsStatementForPassedQuery() 
        {
            var engine = new Mock<IQueryEngine>();
            engine.Setup(e => e.ExecuteQuery(It.IsAny<Query>()))
                .Returns<Query>((qry) => new StatementResult(new QueryResult(qry)));
            var lre = new LessonResultEvaluator(engine.Object);
            var initialStatement = new Statement() { Content = "Fake Query", LimitResults = true, LessonId = "123" };

            var result = lre.Evaluate(initialStatement);

            Assert.AreEqual(initialStatement.Content, result.Content);
            Assert.AreEqual(initialStatement.LessonId, result.LessonId);
            Assert.AreEqual(initialStatement.LimitResults, result.LimitResults);
        }

        [Test]
        public void EvaluateResultSet_SuccessfulQuery_IsCurrentlyEvaluatedAsGood()
        {
            var engine = new Mock<IQueryEngine>();
            var lre = new LessonResultEvaluator(engine.Object);
            var statement = new Statement();
            var queryResult = new QueryResult() { ExecutionStatus = QueryExecutionStatus.Success };

            var result = lre.EvaluateResultSet(statement, queryResult);

            Assert.IsTrue(result);
        }

        [Test]
        public void EvaluateResultSet_QueryWithError_IsEvaluatedAsNotGood()
        {
            var engine = new Mock<IQueryEngine>();
            var lre = new LessonResultEvaluator(engine.Object);
            var statement = new Statement();
            var queryResult = new QueryResult() { ExecutionStatus = QueryExecutionStatus.Error };

            var result = lre.EvaluateResultSet(statement, queryResult);

            Assert.IsFalse(result);
        }
 
    }
}
