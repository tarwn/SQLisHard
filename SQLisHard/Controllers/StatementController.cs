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
			var result = _exerciseEvaluator.Evaluate(value);
			
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
    }
}