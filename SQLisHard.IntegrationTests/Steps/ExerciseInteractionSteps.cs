using SQLisHard.IntegrationTests.Configs;
using SQLisHard.IntegrationTests.PageLibrary.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SQLisHard.IntegrationTests.Steps
{
	[Binding]
	public class ExerciseInteractionSteps : StepsBase
	{
		[Then(@"the (\d+)(?:st|nd|rd|th) entry on the Exercise list is selected")]
		public void ThenTheStEntryOnTheExerciseListIsSelected(int indexOfSelectedEntry)
		{
			CurrentPage.As<ExercisePage>().AssertNthExerciseIsSelected(indexOfSelectedEntry);
		}

		[When(@"I complete the first exercise")]
		public void WhenICompleteTheFirstExercise()
		{
			CurrentPage.As<ExercisePage>().ExecuteQueryAndWaitForResults(Settings.CurrentSettings.FirstExerciseQuery);
		}
	}
}
