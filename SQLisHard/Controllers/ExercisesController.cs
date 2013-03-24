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

		public ExercisesController() : this(new TemporaryExerciseStore()) { }
		public ExercisesController(IExerciseStore exerciseStore)
		{
			_exerciseStore = exerciseStore;
		}

		[HttpGet]
		public ExerciseSet List(string set = "")
		{
			var definedExercise = _exerciseStore.GetList("");
			return new ExerciseSet(definedExercise);
		}
	}

	public class TemporaryExerciseStore : IExerciseStore
	{
		public static Dictionary<string, DefinedExerciseResult> ResultsCache = new Dictionary<string, DefinedExerciseResult>();

		private static DefinedExerciseSet _hardcodedExercises = new DefinedExerciseSet() {
			Title = "SELECT Exercises",
			Summary = "<p>Whether you're just getting started or trying to brush up on existing skills, this is a great spot to start.</p>",
			Exercises = new List<DefinedExercise>(){
				new DefinedExercise(){ Id = "S1.0",  Title = "S1.0 - SELECT", Details = "<p>Databases store data in tables, which we can think of like spreadsheets. Each table holds rows of data that share a common set of columns. The <em>SELECT</em> statement is used to retrieve rows of data from those tables. </p> <code>SELECT * FROM <i>table_name</i></code><p>Try using the <em>SELECT</em> statement to retrieve rows from the <em>Customers</em> table.</p>", Query = "SELECT * FROM Customers" },
				new DefinedExercise(){ Id = "S1.1",  Title = "S1.1 - SELECT Columns", Details = "<p>The * means \"return all of the columns\". We can also ask for specific columns like so:</p><code> SELECT <i>column_name, column_name</i> FROM <i>table_name</i></code> <p>Try editing your query to retrieve only the <em>Id</em> and <em>FirstName</em> columns.", Query = "SELECT Id, FirstName FROM Customers" },
				new DefinedExercise(){ Id = "S1.1B", Title = "S1.1 - SELECT Columns", Details = "<p>Excellent.</p> <p>Asking for specific columns not only means less data to download, it also means the database won't have to do the extra work of looking the columns up before running your query (like it does with the *).</p><p>Specifying the columns also puts us in control of the order they are received. Try to retrieve the <em>Id</em> and <FirstName</em> columns in reverse order.</p>", Query = "SELECT FirstName, Id FROM Customers" }
			},
			Finale = new DefinedFinale() { Title = "Finale", Details = "All Done!" }
		};

		public DefinedExerciseSet GetList(string exerciseSetId)
		{
			return _hardcodedExercises;
		}

		public DefinedExerciseResult GetExerciseResultForComparison(string exerciseId)
		{
			var exercise = _hardcodedExercises.Exercises.Where(e => e.Id == exerciseId).FirstOrDefault();
			var query = new Query() { 
				Content = exercise.Query,
				LimitResults = true
			};
			var db = new QueryEngine(ConfigurationManager.ConnectionStrings["SampleDatabase"].ConnectionString);
			var result = db.ExecuteQuery(query);
			return new DefinedExerciseResult(result);
		}
	}
}