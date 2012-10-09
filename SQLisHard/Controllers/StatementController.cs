using SQLisHard.Domain;
using SQLisHard.Domain.DatabaseExecution;
using SQLisHard.Domain.LessonEvaluator;
using SQLisHard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SQLisHard.Controllers
{
    public class StatementController : ApiController
    {
        private ILessonResultEvaluator _lessonEvaluator;

        public StatementController() : this(new LessonResultEvaluator(new QueryEngine())) { }
        public StatementController(ILessonResultEvaluator evaluator)
        {
            _lessonEvaluator = evaluator;
        }


        // POST api/<controller>
        public StatementResult Post([FromBody]Statement value)
        {
            return _lessonEvaluator.Evaluate(value);
        }

    }
}