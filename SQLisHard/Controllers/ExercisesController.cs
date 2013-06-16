using SQLisHard.Domain.Exercises;
using SQLisHard.Domain.QueryEngine;
using SQLisHard.Domain.QueryEngine.DatabaseExecution;
using SQLisHard.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SQLisHard.Controllers
{
	public class ExercisesController : ApiController
	{
		IExerciseStore _exerciseStore;

		public ExercisesController() : this(MvcApplication.ExerciseStore) { }
		public ExercisesController(IExerciseStore exerciseStore)
		{
			_exerciseStore = exerciseStore;
		}

		[HttpGet]
		public ExerciseSet List(string exerciseSetId = "")
		{
			var definedExercise = _exerciseStore.GetList(exerciseSetId);
			return new ExerciseSet(definedExercise);
		}
	}

}