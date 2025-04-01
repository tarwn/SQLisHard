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
		public void GetList_NonexistentSet_ThrowsException()
		{
			var store = new FlatFileExerciseStore(new Mock<IQueryEngine>().Object);

			Assert.Throws<ArgumentOutOfRangeException>(() => store.GetList("bad set id"));
		}

		[Test]
		public void GetList_ValidSet_ReturnsRespectiveSet()
		{
			var store = new FlatFileExerciseStore(new Mock<IQueryEngine>().Object);
			var sampleSet = new DefinedExerciseSet("SampleId");
			store.Add(sampleSet);

			var result = store.GetList("SampleId");

			Assert.That(result, Is.EqualTo(sampleSet));
		}

		[Test]
		public void GetExerciseResultForComparison_UnknownExercise_ReturnsNullResult()
		{
			var store = new FlatFileExerciseStore(new Mock<IQueryEngine>().Object);

			var result = store.GetExerciseResultForComparison("NonexistentId", "NonexistentId");

			Assert.That(result, Is.Null);
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

			engine.Verify(e => e.ExecuteQuery(It.Is<Query>(q => q.Content == sampleQuery), It.IsAny<bool>()));
		}

		[Test]
		public void ParseFile_ValidFile_ParsesSetProperties()
		{
			var data = GenerateSampleData(setId: "SampleSetId", setTitle: "Set Title", setSummary: "Summary of set");

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.That(result.Id, Is.EqualTo("SampleSetId"));
			Assert.That(result.Title, Is.EqualTo("Set Title"));
			Assert.That(result.Summary, Is.EqualTo("Summary of set"));
		}

		[Test]
		public void ParseFile_ValidFile_ParsesSetPropertiesWithoutInitialWhitespace()
		{
			var data = GenerateSampleData(setId: " SampleSetId", setTitle: " Set Title", setSummary: " Summary of set");

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.That(result.Id, Is.EqualTo("SampleSetId"));
			Assert.That(result.Title, Is.EqualTo("Set Title"));
			Assert.That(result.Summary, Is.EqualTo("Summary of set"));
		}

		[Test]
		public void ParseFile_ValidFile_ParsesMultilineSummaryWithoutExtraTabs()
		{
			var data = GenerateSampleData(setSummary: "Summary of set\n\trest of summary");

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.That(result.Summary, Is.EqualTo("Summary of set\n rest of summary"));
		}

		[Test]
		public void ParseFile_ValidFile_ParsesExerciseSetFinale()
		{
			var data = GenerateSampleData(finaleTitle: "Finale Title", finaleDetails: "finale detail");

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.That(result.Finale.Title, Is.EqualTo("Finale Title"));
			Assert.That(result.Finale.Details, Is.EqualTo("finale detail"));
		}

		[Test]
		public void ParseFile_ValidFile_ParsesExerciseSetFinaleWithMultilineDetails()
		{
			var details = "Finale\n\twith\n\tmultiple lines";
			var data = GenerateSampleData(finaleDetails: details);

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.That(result.Finale.Details, Is.EqualTo(details.Replace("\t"," ")));
		}

		[Test]
		public void ParseFile_FileWithEmptyId_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(setId: "");

			Assert.Throws<DefinedExerciseFileFormatException>(() => FlatFileExerciseStore.ParseFile(data));
		}

		[Test]
		public void ParseFile_FileWithEmptySetTitle_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(setTitle: "");

			Assert.Throws<DefinedExerciseFileFormatException>(() => FlatFileExerciseStore.ParseFile(data));
		}
		
		[Test]
		public void ParseFile_FileWithEmptySetSummary_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(setSummary: "");

			Assert.Throws<DefinedExerciseFileFormatException>(() => FlatFileExerciseStore.ParseFile(data));
		}

		[Test]
		public void ParseFile_FileWithEmptyFinaleTitle_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(finaleTitle: "");

			Assert.Throws<DefinedExerciseFileFormatException>(() => FlatFileExerciseStore.ParseFile(data));
		}

		[Test]
		public void ParseFile_FileWithEmptyFinaleDetails_ThrowsFileFormatException()
		{
			var data = GenerateSampleData(finaleDetails: "");

			Assert.Throws<DefinedExerciseFileFormatException>(() => FlatFileExerciseStore.ParseFile(data));
		}

		[Test]
		public void ParseFile_WithValidBasicExercise_ParsesExpectedNumberOfExercises()
		{
			var data = GenerateSampleData(exerciseCount: 1);

			var result = FlatFileExerciseStore.ParseFile(data);

			Assert.That(result.Exercises.Count, Is.EqualTo(1));
		}

		[Test]
		public void ParseFile_WithValidBasicExercise_ParsesFieldsCorrectly()
		{
			var data = GenerateSampleData(exerciseCount: 1);

			var result = FlatFileExerciseStore.ParseFile(data);

			// these are comparisons to the hard-coded values below
			var ex = result.Exercises.First();
			Assert.That(ex.Id, Is.EqualTo("0"));
			Assert.That(ex.Title, Is.EqualTo("Exercise #0"));
			Assert.That(ex.Query, Is.EqualTo("SELECT TOP 0 FROM SampleTable;"));
			Assert.That(ex.Explanation, Is.EqualTo("This is the multi-line\n\tdescription".Replace("\t", " ")));
			Assert.That(ex.Example, Is.EqualTo("<code>with some code</code>".Replace("\t", " ")));
			Assert.That(ex.Exercise, Is.EqualTo("<p>Multi\n\tline exercise".Replace("\t", " ").TrimEnd()));
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
