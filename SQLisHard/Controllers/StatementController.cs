using SQLisHard.Core;
using SQLisHard.Domain;
using SQLisHard.Domain.LessonEvaluator;
using SQLisHard.Domain.QueryEngine.DatabaseExecution;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
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


        // POST api/<controller>
        public StatementResult Post([FromBody]Statement value)
        {
			value.RequestorId = new UserId();
            return _lessonEvaluator.Evaluate(value);
        }

    }
}