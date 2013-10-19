using Moq;
using NUnit.Framework;
using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Core.Models;
using SQLisHard.Domain.ExerciseEvaluator;
using SQLisHard.Domain.Exercises;
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
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>(), It.IsAny<bool>()))
							   .Returns<Query, bool>((qry, useStats) => new StatementResult(new QueryResult(qry)));
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
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>(), It.IsAny<bool>()))
							   .Returns<Query, bool>((qry, useStats) => new StatementResult(new QueryResult(qry)));
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

		[Test]
		public void Evaluate_UserQueryMatchingPatternAndQuery_IsEvaluatedAsGood()
		{
			string exerciseId = "123";
			string pattern = "SELECT \\* FROM TableName ([^\\s]+) WHERE \\1\\.Column = 5";
			string query = "SELECT * FROM TableName TN WHERE TN.Column = 5";
			var lre = new ExerciseResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>(), It.IsAny<bool>()))
							   .Returns<Query, bool>((qry, useStats) => new StatementResult(new QueryResult(qry)));
			lre.MockExerciseStore.Setup(e => e.GetExercise("ABC", exerciseId))
								 .Returns(new DefinedExercise(exerciseId) { Query = "Fake Query", Pattern = pattern });
			var initialStatement = new Statement() { ExerciseSetId = "ABC", ExerciseId = exerciseId, Content = query, LimitResults = true, RequestorId = new UserId() };

			var queryResult = lre.InstanceUnderTest.Evaluate(initialStatement);

			Assert.IsTrue(queryResult.CompletesExercise);
		}

		[Test]
		public void Evaluate_UserQueryMatchingPatternAndQuery_StillPerformsResultComparison()
		{
			string exerciseId = "123";
			string pattern = "SELECT \\* FROM TableName ([^\\s]+) WHERE \\1\\.Column = 5";
			string query = "SELECT * FROM TableName TN WHERE TN.Column = 5";
			var lre = new ExerciseResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>(), It.IsAny<bool>()))
							   .Returns<Query, bool>((qry, useStats) => new StatementResult(new QueryResult(qry)));
			lre.MockExerciseStore.Setup(e => e.GetExercise("ABC", exerciseId))
								 .Returns(new DefinedExercise(exerciseId) { Query = "Fake Query", Pattern = pattern });
			var initialStatement = new Statement() { ExerciseSetId = "ABC", ExerciseId = exerciseId, Content = query, LimitResults = true, RequestorId = new UserId() };

			var queryResult = lre.InstanceUnderTest.Evaluate(initialStatement);

			lre.MockExerciseStore.Verify(es => es.GetExerciseResultForComparison(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
		}

		[Test]
		public void Evaluate_UserQueryNotMatchingPattern_IsEvaluatedAsNotGood()
		{
			string exerciseId = "123";
			string pattern = "SELECT * FROM TableName ([^\\s]+) WHERE \\1.Column = 5";
			string query = "SELECT * FROM TableName WHERE Column = 5";
			var lre = new ExerciseResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>(), It.IsAny<bool>()))
							   .Returns<Query, bool>((qry, useStats) => new StatementResult(new QueryResult(qry)));
			lre.MockExerciseStore.Setup(e => e.GetExercise("ABC", exerciseId))
								 .Returns(new DefinedExercise(exerciseId) { Query = "Fake Query", Pattern = pattern });
			var initialStatement = new Statement() { ExerciseSetId = "ABC", ExerciseId = exerciseId, Content = query, LimitResults = true, RequestorId = new UserId() };


			var queryResult = lre.InstanceUnderTest.Evaluate(initialStatement);

			Assert.IsFalse(queryResult.CompletesExercise);
		}

		[Test]
		public void Evaluate_UserQueryNotMatchingPattern_ReturnsTipAssociatedWithPattern()
		{
			string exerciseId = "123";
			string pattern = "SELECT * FROM TableName ([^\\s]+) WHERE \\1.Column = 5";
			string query = "SELECT * FROM TableName WHERE Column = 5";
			string patternTip = "That was close, make sure you do the thing with other thing next time";
			var lre = new ExerciseResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>(), It.IsAny<bool>()))
							   .Returns<Query,bool>((qry, useStats) => new StatementResult(new QueryResult(qry)));
			lre.MockExerciseStore.Setup(e => e.GetExercise("ABC", exerciseId))
								 .Returns(new DefinedExercise(exerciseId) { Query = "Fake Query", Pattern = pattern, PatternTip = patternTip });
			var initialStatement = new Statement() { ExerciseSetId = "ABC", ExerciseId = exerciseId, Content = query, LimitResults = true, RequestorId = new UserId() };


			var queryResult = lre.InstanceUnderTest.Evaluate(initialStatement);

			Assert.AreEqual(patternTip, queryResult.Tip);
		}

		[Test]
		public void Evaluate_UserQueryNotMatchingPattern_DoesNotPerformResultComparison()
		{
			string exerciseId = "123";
			string pattern = "SELECT * FROM TableName ([^\\s]+) WHERE \\1.Column = 5";
			string query = "SELECT * FROM TableName WHERE Column = 5";
			var lre = new ExerciseResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>(), It.IsAny<bool>()))
							   .Returns<Query,bool>((qry, useStats) => new StatementResult(new QueryResult(qry)));
			lre.MockExerciseStore.Setup(e => e.GetExercise("ABC", exerciseId))
								 .Returns(new DefinedExercise(exerciseId) { Query = "Fake Query", Pattern = pattern });
			var initialStatement = new Statement() { ExerciseSetId = "ABC", ExerciseId = exerciseId, Content = query, LimitResults = true, RequestorId = new UserId() };


			var queryResult = lre.InstanceUnderTest.Evaluate(initialStatement);

			lre.MockExerciseStore.Verify(es => es.GetExerciseResultForComparison(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void Evaluate_UserQueryNotMatchingPattern_AddsHistoryEntry()
		{
			string exerciseId = "123";
			string pattern = "SELECT * FROM TableName ([^\\s]+) WHERE \\1.Column = 5";
			string query = "SELECT * FROM TableName WHERE Column = 5";
			var lre = new ExerciseResultEvaluatorHarness();
			lre.MockQueryEngine.Setup(e => e.ExecuteQuery(It.IsAny<Query>(), It.IsAny<bool>()))
							   .Returns<Query, bool>((qry, useStats) => new StatementResult(new QueryResult(qry)));
			lre.MockExerciseStore.Setup(e => e.GetExercise("ABC", exerciseId))
								 .Returns(new DefinedExercise(exerciseId) { Query = "Fake Query", Pattern = pattern });
			var initialStatement = new Statement() { ExerciseSetId = "ABC", ExerciseId = exerciseId, Content = query, LimitResults = true, RequestorId = new UserId() };


			var queryResult = lre.InstanceUnderTest.Evaluate(initialStatement);

			lre.MockHistoryStore.Verify(hs => hs.AddToHistory(initialStatement.RequestorId, initialStatement.Content, (int)queryResult.ExecutionStatus, false), Times.Once());
		}

	}

	public class ExerciseResultEvaluatorHarness
	{
		public Mock<IQueryEngine> MockQueryEngine { get; set; }
		public Mock<IExerciseStore> MockExerciseStore { get; set; }
		public Mock<IHistoryStore> MockHistoryStore { get; set; }
		public ExerciseResultEvaluator InstanceUnderTest { get; set; }

		public ExerciseResultEvaluatorHarness()
		{
			MockQueryEngine = new Mock<IQueryEngine>();
			MockExerciseStore = new Mock<IExerciseStore>();
			MockExerciseStore.Setup(me => me.GetExerciseResultForComparison(It.IsAny<string>(), It.IsAny<string>())).Returns(new DefinedExerciseResultFake(true));
			MockExerciseStore.Setup(me => me.GetExercise(It.IsAny<string>(), It.IsAny<string>()))
							 .Returns<string,string>((setId, id) => new DefinedExercise(id));
			MockHistoryStore = new Mock<IHistoryStore>();
			InstanceUnderTest = new ExerciseResultEvaluator(MockQueryEngine.Object, MockExerciseStore.Object, MockHistoryStore.Object);
		}

	}

	public class DefinedExerciseResultFake : DefinedExerciseResult
	{
		bool _expectedEqualsValue;

		public DefinedExerciseResultFake(bool expectedEqualsValue) 
		{
			_expectedEqualsValue = expectedEqualsValue;
		}

		public override bool Equals(object obj)
		{
			return _expectedEqualsValue;
		}
	}
}
