using Moq;
using NUnit.Framework;
using SQLisHard.Controllers;
using SQLisHard.Core;
using SQLisHard.Core.Models;
using SQLisHard.Domain.ExerciseEvaluator;
using SQLisHard.Domain.QueryEngine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using System.Collections.Generic;

namespace SQLisHard.Tests.Controllers
{
	[TestFixture]
	public class StatementControllerTests
	{
		[Test]
		public void Post_ValidStatement_ReturnsResultFromExerciseEvaluator()
		{
			var statement = new Statement() { Content = "SELECT * FROM SampleTable" };
			var mockEvaluator = new Mock<IExerciseResultEvaluator>();
			mockEvaluator.Setup(e => e.Evaluate(It.IsAny<Statement>()))
						 .Returns<Statement>((s) => new StatementResult(new QueryResult(s)) {
							 ExecutionStatus = QueryExecutionStatus.Success,
							 CompletesExercise = true
						 });
			var controller = GetAStatementController(mockEvaluator.Object);

			var actionResult = controller.Post(statement);
			var result = actionResult as OkObjectResult;
			var statementResult = result?.Value as StatementResult;

			mockEvaluator.Verify(e => e.Evaluate(statement));
			Assert.That(statementResult, Is.Not.Null);
		}

		[Test]
		public void Post_EmptyStatement_ReturnsReasonableError()
		{
			var controller = GetAStatementController(Mock.Of<IExerciseResultEvaluator>());
			var statement = new Statement(){ Content = "" };

			var actionResult = controller.Post(statement);
			var result = actionResult as OkObjectResult;
			var statementResult = result?.Value as StatementResult;

			Assert.That(statementResult, Is.Not.Null);
			Assert.That(statementResult!.ExecutionStatus, Is.EqualTo(QueryExecutionStatus.Error));
			Assert.That(statementResult.ErrorMessage, Is.EqualTo(StatementController.ERROR_EMPTY_STATEMENT));
		}

		[Test]
		public void Post_ShowTablesStatement_ReturnsResultsFromInformationSchema()
		{
			var statement = new Statement() { Content = "show tables" };
			var mockEvaluator = new Mock<IExerciseResultEvaluator>();
			mockEvaluator.Setup(e => e.Evaluate(It.IsAny<Statement>()))
						 .Returns<Statement>((s) => new StatementResult(new QueryResult(s)) {
							 ExecutionStatus = QueryExecutionStatus.Success,
							 CompletesExercise = true
						 });
			var controller = GetAStatementController(mockEvaluator.Object);

			var actionResult = controller.Post(statement);
			var result = actionResult as OkObjectResult;
			var statementResult = result?.Value as StatementResult;

			Assert.That(statementResult, Is.Not.Null);
			Assert.That(statementResult!.ExecutionStatus, Is.EqualTo(QueryExecutionStatus.Success));
			Assert.That(statementResult.Content, Is.EqualTo(StatementController.ReplacementQueries.ShowTables));
		}

		[Test]
		public void Post_HelpStatement_ReturnsResultsFromInformationSchema()
		{
			var statement = new Statement() { Content = "help " };
			var mockEvaluator = new Mock<IExerciseResultEvaluator>();
			mockEvaluator.Setup(e => e.Evaluate(It.IsAny<Statement>()))
						 .Returns<Statement>((s) => new StatementResult(new QueryResult(s)) {
							 ExecutionStatus = QueryExecutionStatus.Success,
							 CompletesExercise = true
						 });
			var controller = GetAStatementController(mockEvaluator.Object);

			var actionResult = controller.Post(statement);
			var result = actionResult as OkObjectResult;
			var statementResult = result?.Value as StatementResult;

			Assert.That(statementResult, Is.Not.Null);
			Assert.That(statementResult!.ExecutionStatus, Is.EqualTo(QueryExecutionStatus.Success));
			Assert.That(statementResult.Content, Is.EqualTo(StatementController.ReplacementQueries.Help));
		}

		private static StatementController GetAStatementController(IExerciseResultEvaluator evaluator)
		{
			var controller = new StatementController(evaluator);
			
			// Setup HttpContext with claims
			var claims = new List<Claim>
			{
				new Claim("id", "123")
			};
			var identity = new ClaimsIdentity(claims);
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext
			{
				User = principal
			};
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};
			
			return controller;
		}
	}
}
