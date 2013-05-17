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
				new DefinedExercise(){ Id = "S1.0",  Title = "S1.0 - SELECT", Details = "<p>Databases store data in tables, which we can think of like spreadsheets. Each table holds rows of data that share a common set of columns. We use the <em>SELECT</em> statement to retrieve rows of data from those tables. </p> <code>SELECT *<br/> FROM <i>table_name</i></code><p>Try using the <em>SELECT</em> statement to retrieve rows from the <em>Customers</em> table.</p>", Query = "SELECT * FROM Customers" },
				new DefinedExercise(){ Id = "S1.1",  Title = "S1.1 - SELECT Columns", Details = "<p>The * means \"return all of the columns\". We can also ask for specific columns:</p><code> SELECT <i>column_name, column_name</i><br/> FROM <i>table_name</i></code> <p>Try editing your query to retrieve only the <em>Id</em> and <em>FirstName</em> columns.", Query = "SELECT Id, FirstName FROM Customers" },
				new DefinedExercise(){ Id = "S1.2", Title = "S1.2 - SELECT Columns", Details = "<p>Asking for specific columns means less data to download and gives us control of the order the columns will be returned. It also means the database won't have to lookup all the columns before executing the query, like it does when we SELECT *.</p><p>SELECT the <em>Id</em> and <em>FirstName</em> columns from the <em>Customers</e> in reverse order.</p>", Query = "SELECT FirstName, Id FROM Customers" },
				new DefinedExercise(){ Id = "S2.0", Title = "S2.0 - WHERE", Details = "<p>Now we can read all of the rows from a table, but typically we only want to read rows that meet certain criteria. We can use the WHERE clause to suplpy those criteria, like so:</p><code> SELECT <i>column_name, column_name</i><br/>FROM <i>table_name</i><br/>WHERE <i>condition</i></code><p>A numeric condition to filter on id's that are equal to 5 would look like: <em>id = 5</em>. Let's SELECT all columns (*) FROM the <em>Customers</em> table using that example condition.</p>", Query = "SELECT * FROM Customers WHERE Id = 5" },
				new DefinedExercise(){ Id = "S2.1", Title = "S2.1 - WHERE conditions", Details = "<p>Besides equality comparisons, we can also make greater than &gt;, less than &lt;, not equals &lt;&gt;, and BETWEEN comparisons.</p><code> SELECT <i>column_name, column_name</i><br/>FROM <i>table_name</i><br/>WHERE <i>column_name</i> BETWEEN <i>value1</i> AND <i>value2</i></code><p>Let's select all columns (*) from <em>Orders</em> that have an <em>OrderTotal</em> BETWEEN 100 and 200</p>", Query = "SELECT * FROM Orders WHERE OrderTotal BETWEEN 100 and 200" },
				new DefinedExercise(){ Id = "S2.2", Title = "S2.2 - WHERE conditions", Details = "<p>Conditions can be applied to any column in our tables, not just numbers. When we compare date and text vaules we ave to surround them with single quotes.</p><code>SELECT *<br/>FROM Orders<br/>WHERE OrderTime > '2013-01-01'</code><p>Let's select all columns (*) from <em>Orders</em> where the <em>DeliveryTime</em> is less than June 20th, 2013.</p>", Query = "SELECT * FROM Orders WHERE DeliveryTime < '2013-06-20'" },
				new DefinedExercise(){ Id = "S2.3", Title = "S2.3 - WHERE LIKE", Details = "<p>The LIKE comparison allows us to search for text that matches a partial search string. For instance, searching for Name LIKE 'Jan%' would match Jane and Janet but not Jack. The % character is a wildcard that can match any number of unknown characters and the _ character will match a single unknown character.</p><code>SELECT *<br/>FROM Customers<br/>WHERE FirstName LIKE 'Jan%'</code><p>Let's select all columns (*) from <em>Customers</em> whose <em>LastName</em> starts with the letter A.</p>", Query = "SELECT * FROM Customers WHERE LastName LIKE 'A%'" },
				new DefinedExercise(){ Id = "S2.4", Title = "S2.4 - WHERE OR", Details = "<p>We can combine conditions with AND and OR statements to define more complex conditions.</p><code>SELECT *<br/>FROM Customers<br/>WHERE Id = 1 OR Id = 12</code><p>Let's select all columns (*) from <em>Customers</em> where their <em>FirstName</em> starts with A or their <em>Id</em> is less than 10.</p>", Query = "SELECT * FROM Customers WHERE FirstName LIKE 'A%' OR Id < 10" },
				new DefinedExercise(){ Id = "S4.0", Title = "S4.0 - INNER JOIN", Details = "<p>Now that we can pull data out of a single table, let's take it a step further. JOIN statements allow us to 'join' the records of several tables together with instructions on how to match up the records in each</p><code>SELECT <i>[columns]</i><br/>FROM <i>table_name</i><br/>INNER JOIN <i>table_name2</i> ON <i>condition</i></code><p>Try using the INNER JOIN syntax to SELECT all columns from the <em>Customers</em> and <em>Orders</em> tables where the <em>CustomerId</em> column in <em>Orders</em> matches the <em>Id</em> column in <em>Customers</em>.</p>", Query = "SELECT * FROM Customers C INNER JOIN Orders O ON C.id = O.CustomerId" }
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
			if (exercise == null)
				return null;

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