using Moq;
using NUnit.Framework;
using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Core.Models;
using SQLisHard.Domain.ExerciseEvaluator;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain.Tests.ExerciseEvaluator
{
	[TestFixture]
	public class ExerciseResultEvaluatorTests
	{

		[Test]
		public void Evaluate_Statement_ReturnsStatementForPassedQuery()
		{
			var lre = new ExerciseResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>()))
							   .Returns<Query>((qry) => new StatementResult(new QueryResult(qry)));
			var initialStatement = new Statement() { Content = "Fake Query", LimitResults = true, ExerciseId = "123" };

			var result = lre.InstanceUnderTest.Evaluate(initialStatement);

			Assert.AreEqual(initialStatement.Content, result.Content);
			Assert.AreEqual(initialStatement.ExerciseId, result.ExerciseId);
			Assert.AreEqual(initialStatement.LimitResults, result.LimitResults);
		}

		[Test]
		public void Evaluate_SuccessfulQuery_IsAddedToHistory()
		{
			var lre = new ExerciseResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>()))
							   .Returns<Query>((qry) => new StatementResult(new QueryResult(qry)));
			var initialStatement = new Statement() { Content = "Fake Query", LimitResults = true, ExerciseId = "123", RequestorId = new UserId() };

			var queryResult = lre.InstanceUnderTest.Evaluate(initialStatement);

			lre.MockHistoryStore.Verify(hs => hs.AddToHistory(initialStatement.RequestorId, initialStatement.Content, (int)queryResult.ExecutionStatus, true), Times.Once());
		}

		[Test]
		public void EvaluateResultSet_SuccessfulQuery_IsCurrentlyEvaluatedAsGood()
		{
			var lre = new ExerciseResultEvaluatorHarness();
			var statement = new Statement();
			var queryResult = new QueryResult() { ExecutionStatus = QueryExecutionStatus.Success };

			var result = lre.InstanceUnderTest.EvaluateResultSet(statement, queryResult);

			Assert.IsTrue(result);
		}

		[Test]
		public void EvaluateResultSet_QueryWithError_IsEvaluatedAsNotGood()
		{
			var lre = new ExerciseResultEvaluatorHarness();
			var statement = new Statement();
			var queryResult = new QueryResult() { ExecutionStatus = QueryExecutionStatus.Error };

			var result = lre.InstanceUnderTest.EvaluateResultSet(statement, queryResult);

			Assert.IsFalse(result);
		}

	}

	public class ExerciseResultEvaluatorHarness
	{
		public Mock<IQueryEngine> MockQueryEngine { get; set; }
		public Mock<IHistoryStore> MockHistoryStore { get; set; }
		public ExerciseResultEvaluator InstanceUnderTest { get; set; }

		public ExerciseResultEvaluatorHarness()
		{
			MockQueryEngine = new Mock<IQueryEngine>();
			MockHistoryStore = new Mock<IHistoryStore>();
			InstanceUnderTest = new ExerciseResultEvaluator(MockQueryEngine.Object, MockHistoryStore.Object);
		}

	}
}
