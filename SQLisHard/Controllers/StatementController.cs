using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLisHard.Core.Models;
using SQLisHard.Domain.ExerciseEvaluator;

namespace SQLisHard.Controllers;

	[Route("api/[controller]")]
    public class StatementController : Controller
    {
        private readonly IExerciseResultEvaluator _exerciseEvaluator;

		public const string ERROR_EMPTY_STATEMENT = "Error, you have not entered a query to execute. Please type one in before using the Execute button to run it.";

        public StatementController(IExerciseResultEvaluator evaluator)
        {
			_exerciseEvaluator = evaluator;
        }

		[HttpPost]
		[Authorize]
		[UpperCaseJSONOutput]
        public IActionResult Post([FromBody]Statement value)
        {
			var id = int.Parse(User.FindFirstValue("id")!);
			value.RequestorId = new UserId(id);

			var result = ValidateAndExecute(value, _exerciseEvaluator.Evaluate);
			
			// TODO - I think this was powering some of the experience tracking and should be redone differently now
			this.HttpContext.Items.Add("AdditionalInteractionValues", new Dictionary<string, string>() { 
				{"ExerciseId", value.ExerciseId},
				{"ResultStatus", result.ExecutionStatus.ToString() },
				{"CompletesExercise", result.CompletesExercise.ToString() }
			});

			return Ok(result);
        }

		[HttpGet("exception")]
		public IActionResult GetException()
		{
			throw new Exception("WebAPI Exception message");
		}

		private StatementResult ValidateAndExecute(Statement statement, Func<Statement, StatementResult> evaluateStatement)
		{
			if (String.IsNullOrWhiteSpace(statement.Content))
				return new StatementResult(statement, ERROR_EMPTY_STATEMENT);

			statement = FilterForReplacements(statement);

			return evaluateStatement(statement);
		}

		private Statement FilterForReplacements(Statement originalStatement) 
		{
			if (originalStatement.Content.StartsWith("show tables", StringComparison.CurrentCultureIgnoreCase))
				originalStatement.Content = ReplacementQueries.ShowTables;
			else if (originalStatement.Content.StartsWith("help", StringComparison.CurrentCultureIgnoreCase))
				originalStatement.Content = ReplacementQueries.Help;

			return originalStatement;
		}

		public static class ReplacementQueries 
		{
			public const string ShowTables = "/* MySQL commands not supported, try the ANSI-standard INFORMATION_SCHEMA instead: http://en.wikipedia.org/wiki/Information_schema */\n\nSELECT * FROM INFORMATION_SCHEMA.TABLES";
			public const string Help = "SELECT 'Sorry, there is no command-line help command.' as [help]";
		}
    }
