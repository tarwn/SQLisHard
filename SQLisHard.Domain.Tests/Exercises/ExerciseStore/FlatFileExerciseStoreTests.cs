using Moq;
using NUnit.Framework;
using SQLisHard.Domain.Exercises;
using SQLisHard.Domain.Exercises.ExerciseStore;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain.Tests.Exercises.ExerciseStore
{
	[TestFixture]
	public class FlatFileExerciseStoreTests
	{

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetList_NonexistentSet_ThrowsException()
		{
			var store = new FlatFileExerciseStore(new Mock<IQueryEngine>().Object);

			var result = store.GetList("bad set id");

			// expect exception
		}

		[Test]
		public void GetList_ValidSet_ReturnsRespectiveSet()
		{
			var store = new FlatFileExerciseStore(new Mock<IQueryEngine>().Object);
			var sampleSet = new DefinedExerciseSet("SampleId");
			store.Add(sampleSet);

			var result = store.GetList("SampleId");

			Assert.AreEqual(sampleSet, result);
		}

		[Test]
		public void GetExerciseResultForComparison_UnknownExercise_ReturnsNullResult()
		{
			var store = new FlatFileExerciseStore(new Mock<IQueryEngine>().Object);

			var result = store.GetExerciseResultForComparison("NonexistentId", "NonexistentId");

			Assert.IsNull(result);
		}

		[Test]
		public void GetExerciseResultForComparison_KnownExercise_UsesEngineToExecuteQueryForResult()
		{
			var engine = new Mock<IQueryEngine>();
			var store = new FlatFileExerciseStore(engine.Object);
			string sampleSetId = "SampleSetId";
			string sampleExerciseId = "SampleExerciseId";
			string sampleQuery = "THIS IS THE SAMPLE QUERY";
			var sampleSet = new DefinedExerciseSet(sampleSetId) {
				Exercises = new List<DefinedExercise>() { 
					new DefinedExercise(sampleExerciseId){ Query = sampleQuery }
				}
			};
			store.Add(sampleSet);

			var result = store.GetExerciseResultForComparison(sampleSetId, sampleExerciseId);

			engine.Verify(e => e.ExecuteQuery(It.Is<Query>(q => q.Content == sampleQuery)));
		}

		[Test]
		public void ParseFile_ValidFile_ParsesSetProperties()
		{
			var data = GenerateSampleData(setId: "SampleSetId", setTitle: "Set Title", setSummary: "Summary of set");

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.AreEqual("SampleSetId", result.Id);
			Assert.AreEqual("Set Title", result.Title);
			Assert.AreEqual("Summary of set", result.Summary);
		}

		[Test]
		public void ParseFile_ValidFile_ParsesSetPropertiesWithoutInitialWhitespace()
		{
			var data = GenerateSampleData(setId: " SampleSetId", setTitle: " Set Title", setSummary: " Summary of set");

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.AreEqual("SampleSetId", result.Id);
			Assert.AreEqual("Set Title", result.Title);
			Assert.AreEqual("Summary of set", result.Summary);
		}

		[Test]
		public void ParseFile_ValidFile_ParsesMultilineSummaryWithoutExtraTabs()
		{
			var data = GenerateSampleData(setSummary: "Summary of set\n\trest of summary");

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.AreEqual("Summary of set\n rest of summary", result.Summary);
		}

		[Test]
		public void ParseFile_ValidFile_ParsesExerciseSetFinale()
		{
			var data = GenerateSampleData(finaleTitle: "Finale Title", finaleDetails: "finale detail");

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.AreEqual("Finale Title", result.Finale.Title);
			Assert.AreEqual("finale detail", result.Finale.Details);
		}

		[Test]
		public void ParseFile_ValidFile_ParsesExerciseSetFinaleWithMultilineDetails()
		{
			var details = "Finale\n\twith\n\tmultiple lines";
			var data = GenerateSampleData(finaleDetails: details);

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.AreEqual(details.Replace("\t"," "), result.Finale.Details);
		}

		[Test]
		[ExpectedException(typeof(DefinedExerciseFileFormatException))]
		public void ParseFile_FileWithEmptyId_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(setId: "");

			var result = FlatFileExerciseStore.ParseFile(data);

			//expects exception
		}

		[Test]
		[ExpectedException(typeof(DefinedExerciseFileFormatException))]
		public void ParseFile_FileWithEmptySetTitle_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(setTitle: "");

			var result = FlatFileExerciseStore.ParseFile(data);

			//expects exception
		}
		
		[Test]
		[ExpectedException(typeof(DefinedExerciseFileFormatException))]
		public void ParseFile_FileWithEmptySetSummary_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(setSummary: "");

			var result = FlatFileExerciseStore.ParseFile(data);

			//expects exception
		}

		[Test]
		[ExpectedException(typeof(DefinedExerciseFileFormatException))]
		public void ParseFile_FileWithEmptyFinaleTitle_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(finaleTitle: "");

			var result = FlatFileExerciseStore.ParseFile(data);

			//expects exception
		}

		[Test]
		[ExpectedException(typeof(DefinedExerciseFileFormatException))]
		public void ParseFile_FileWithEmptyFinaleDetails_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(finaleDetails: "");

			var result = FlatFileExerciseStore.ParseFile(data);

			//expects exception
		}

		[Test]
		public void ParseFile_WithValidBasicExercise_ParsesExpectedNumberOfExercises()
		{
			var data = GenerateSampleData(exerciseCount: 1);

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.AreEqual(1, result.Exercises.Count);
		}

		[Test]
		public void ParseFile_WithValidBasicExercise_ParsesFieldsCorrectly()
		{
			var data = GenerateSampleData(exerciseCount: 1);

			var result = FlatFileExerciseStore.ParseFile(data);

			// these are comparisons to the hard-coded values below
			var ex = result.Exercises.First();
			Assert.AreEqual("0", ex.Id);
			Assert.AreEqual("Exercise #0", ex.Title);
			Assert.AreEqual("SELECT TOP 0 FROM SampleTable;", ex.Query);
			Assert.AreEqual("This is the multi-line\n\tdescription".Replace("\t", " "), ex.Explanation);
			Assert.AreEqual("<code>with some code</code>".Replace("\t", " "), ex.Example);
			Assert.AreEqual("<p>Multi\n\tline exercise".Replace("\t", " ").TrimEnd(), ex.Exercise);
		}

		private string GenerateSampleData(string setId = "SampleId", string setTitle = "Sample Title", string setSummary = "Sample Summary",
										  string finaleTitle = "Finale Title", string finaleDetails = "Finale Details",
										  int exerciseCount = 1)
		{
			string data = String.Format(
				"SetId:{0}\n" + 
				"SetTitle:{1}\n" +
				"SetSummary:{2}\n" +
				"FinaleTitle:{3}\n" + 
				"FinaleDetails:{4}\n", 
				setId, setTitle, setSummary, 
				finaleTitle, finaleDetails);

			for (int i = 0; i < exerciseCount; i++)
			{
				data += String.Format(
					"ExerciseId:{0}\n" +
					"Title:Exercise #{0}\n" +
					"Query:SELECT TOP {0} FROM SampleTable;\n" +
					"Explanation:This is the multi-line\n\tdescription\n" + 
					"Example:\t<code>with some code</code>\n" +
					"Exercise:\t<p>Multi\n\tline exercise",
					i);
			}

			return data; 
		}

	}
}
