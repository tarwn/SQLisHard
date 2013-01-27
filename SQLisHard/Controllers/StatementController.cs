using SQLisHard.Attributes;
using SQLisHard.Attributes.WebAPI;
using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Core.Models;
using SQLisHard.Domain;
using SQLisHard.Domain.LessonEvaluator;
using SQLisHard.Domain.QueryEngine.DatabaseExecution;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SQLisHard.Controllers
{
    public class StatementController : ApiController
    {
        private ILessonResultEvaluator _lessonEvaluator;

        public StatementController() : this(
			new LessonResultEvaluator(
				new QueryEngine(ConfigurationManager.ConnectionStrings["SampleDatabase"].ConnectionString), 
				new HistoryStore(ConfigurationManager.ConnectionStrings["CoreDatabase"].ConnectionString))) { }
        public StatementController(ILessonResultEvaluator evaluator)
        {
            _lessonEvaluator = evaluator;
        }

		[RequiresUserOrGuest]
        public StatementResult Post([FromBody]Statement value)
        {
			var user = (UserPrincipal)HttpContext.Current.User;
			value.RequestorId = user.UserIdentity.Id;
            return _lessonEvaluator.Evaluate(value);
        }

		public string GetException()
		{
			throw new Exception("WebAPI Exception message");
		}
    }
}