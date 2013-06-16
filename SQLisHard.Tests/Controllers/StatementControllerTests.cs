using Moq;
using NUnit.Framework;
using SQLisHard.Controllers;
using SQLisHard.Core;
using SQLisHard.Core.Models;
using SQLisHard.Domain.ExerciseEvaluator;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

			var result = controller.Post(statement);

			mockEvaluator.Verify(e => e.Evaluate(statement));
		}

		[Test]
		public void Post_EmptyStatement_ReturnsReasonableError()
		{
			var controller = GetAStatementController();
			var statement = new Statement(){ Content = "" };

			var result = controller.Post(statement);

			Assert.AreEqual(QueryExecutionStatus.Error, result.ExecutionStatus);
			Assert.AreEqual(StatementController.ERROR_EMPTY_STATEMENT, result.ErrorMessage);
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

			var result = controller.Post(statement);

			Assert.AreEqual(QueryExecutionStatus.Success, result.ExecutionStatus);
			Assert.AreEqual(StatementController.ReplacementQueries.ShowTables, result.Content);
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

			var result = controller.Post(statement);

			Assert.AreEqual(QueryExecutionStatus.Success, result.ExecutionStatus);
			Assert.AreEqual(StatementController.ReplacementQueries.Help, result.Content);
		}

		private static StatementController GetAStatementController(IExerciseResultEvaluator evaluator = null)
		{
			evaluator = evaluator ?? new Mock<IExerciseResultEvaluator>().Object;
			var controller = new StatementController(evaluator);
			controller.Request = new System.Net.Http.HttpRequestMessage();
			HttpContext.Current = GetFakeHttpContext();
			HttpContext.Current.User = new UserPrincipal(new GuestUser(new User()));
			return controller;
		}

		public static HttpContext GetFakeHttpContext()
		{
			// adapted from: http://stackoverflow.com/questions/9624242/setting-the-httpcontext-current-session-in-unit-test

			var httpRequest = new HttpRequest("", "http://kindermusik/", "");
			var stringWriter = new StringWriter();
			var httpResponce = new HttpResponse(stringWriter);
			var httpContext = new HttpContext(httpRequest, httpResponce);

			//var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
			//										new HttpStaticObjectsCollection(), 10, true,
			//										HttpCookieMode.AutoDetect,
			//										SessionStateMode.InProc, false);

			//httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
			//							BindingFlags.NonPublic | BindingFlags.Instance,
			//							null, CallingConventions.Standard,
			//							new[] { typeof(HttpSessionStateContainer) },
			//							null)
			//					.Invoke(new object[] { sessionContainer });

			return httpContext;
		}
	}
}
