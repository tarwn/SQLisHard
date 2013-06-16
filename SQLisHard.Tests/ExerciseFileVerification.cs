using NUnit.Framework;
using SQLisHard.Domain.Exercises.ExerciseStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Tests
{
	[TestFixture]
    public class ExerciseFileVerification
    {

		[Test]
		public void EnsureExerciseFilesAreValid()
		{
			var filelist = Directory.EnumerateFiles("../../../SQLisHard/Exercises");
			var errors = new List<string>();
			foreach (var file in filelist)
			{
				try
				{
					var fileContent = File.ReadAllText(file);
					var parsedFile = FlatFileExerciseStore.ParseFile(fileContent);
				}
				catch (Exception exc)
				{
					errors.Add(String.Format("{0}: {1} - {2}", file, exc.GetType().Name, exc.Message));
				}
			}

			if (errors.Count > 0)
				Assert.Fail("Errors parsing one or more files:\n" + String.Join("\n",errors));
		}

    }
}
