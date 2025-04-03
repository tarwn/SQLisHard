using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLisHard.Domain.Exercises;
using SQLisHard.Models;

namespace SQLisHard.Controllers;

	[ApiController]
	[Route("api/[controller]/[action]")]
	[Authorize]
	public class ExercisesController : ControllerBase
	{
		IExerciseStore _exerciseStore;

		public ExercisesController(IExerciseStore exerciseStore)
		{
			_exerciseStore = exerciseStore;
		}

		
		[HttpGet]
		[UpperCaseJSONOutput]
		public ActionResult<ExerciseSet> List(string exerciseSetId = "")
		{
			var definedExercise = _exerciseStore.GetList(exerciseSetId);
			return Ok(new ExerciseSet(definedExercise));
		}
	}

