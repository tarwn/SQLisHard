using SQLisHard.Attributes;
using SQLisHard.Attributes.WebAPI;
using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Core.Models;
using SQLisHard.Domain;
using SQLisHard.Domain.ExerciseEvaluator;
using SQLisHard.Domain.QueryEngine.DatabaseExecution;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SQLisHard.Controllers
{
    public class StatementController : ApiController
    {
        private IExerciseResultEvaluator _exerciseEvaluator;

		public const string ERROR_EMPTY_STATEMENT = "Error, you have not entered a query to execute. Please type one in before using the Execute button to run it.";

        public StatementController() : this(
			new ExerciseResultEvaluator(
				new QueryEngine(ConfigurationManager.ConnectionStrings["SampleDatabase"].ConnectionString), 
				new TemporaryExerciseStore(),
				new HistoryStore(ConfigurationManager.ConnectionStrings["CoreDatabase"].ConnectionString))) { }

        public StatementController(IExerciseResultEvaluator evaluator)
        {
			_exerciseEvaluator = evaluator;
        }

		[RequiresUserOrGuest]
        public StatementResult Post([FromBody]Statement value)
        {
			var user = (UserPrincipal)HttpContext.Current.User;
			value.RequestorId = user.UserIdentity.Id;

			var result = ValidateAndExecute(value, (v) => {
				return _exerciseEvaluator.Evaluate(v);
			});
			
			// hacky: fix later
			this.Request.Properties["AdditionalInteractionValues"] = new Dictionary<string, string>() { 
				{"ExerciseId", value.ExerciseId},
				{"ResultStatus", result.ExecutionStatus.ToString() },
				{"CompletesExercise", result.CompletesExercise.ToString() }
			};

			return result;
        }

		public string GetException()
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
}