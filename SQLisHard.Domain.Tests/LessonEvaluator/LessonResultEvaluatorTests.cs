using Moq;
using NUnit.Framework;
using SQLisHard.Core;
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
			var lre = new LessonResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>()))
							   .Returns<Query>((qry) => new StatementResult(new QueryResult(qry)));
			var initialStatement = new Statement() { Content = "Fake Query", LimitResults = true, LessonId = "123" };

			var result = lre.InstanceUnderTest.Evaluate(initialStatement);

			Assert.AreEqual(initialStatement.Content, result.Content);
			Assert.AreEqual(initialStatement.LessonId, result.LessonId);
			Assert.AreEqual(initialStatement.LimitResults, result.LimitResults);
		}

		[Test]
		public void Evaluate_SuccessfulQuery_IsAddedToHistory()
		{
			var lre = new LessonResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>()))
							   .Returns<Query>((qry) => new StatementResult(new QueryResult(qry)));
			var initialStatement = new Statement() { Content = "Fake Query", LimitResults = true, LessonId = "123", RequestorId = new UserId() };

			var queryResult = lre.InstanceUnderTest.Evaluate(initialStatement);

			lre.MockHistoryStore.Verify(hs => hs.AddToHistory(initialStatement.RequestorId, initialStatement.Content, (int)queryResult.ExecutionStatus, true), Times.Once());
		}

		[Test]
		public void EvaluateResultSet_SuccessfulQuery_IsCurrentlyEvaluatedAsGood()
		{
			var lre = new LessonResultEvaluatorHarness();
			var statement = new Statement();
			var queryResult = new QueryResult() { ExecutionStatus = QueryExecutionStatus.Success };

			var result = lre.InstanceUnderTest.EvaluateResultSet(statement, queryResult);

			Assert.IsTrue(result);
		}

		[Test]
		public void EvaluateResultSet_QueryWithError_IsEvaluatedAsNotGood()
		{
			var lre = new LessonResultEvaluatorHarness();
			var statement = new Statement();
			var queryResult = new QueryResult() { ExecutionStatus = QueryExecutionStatus.Error };

			var result = lre.InstanceUnderTest.EvaluateResultSet(statement, queryResult);

			Assert.IsFalse(result);
		}

	}

	public class LessonResultEvaluatorHarness
	{
		public Mock<IQueryEngine> MockQueryEngine { get; set; }
		public Mock<IHistoryStore> MockHistoryStore { get; set; }
		public LessonResultEvaluator InstanceUnderTest { get; set; }

		public LessonResultEvaluatorHarness()
		{
			MockQueryEngine = new Mock<IQueryEngine>();
			MockHistoryStore = new Mock<IHistoryStore>();
			InstanceUnderTest = new LessonResultEvaluator(MockQueryEngine.Object, MockHistoryStore.Object);
		}

	}
}
