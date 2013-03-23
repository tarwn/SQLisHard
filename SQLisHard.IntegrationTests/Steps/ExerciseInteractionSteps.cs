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
			CurrentPage.As<ExercisePage>().WaitUpTo(1000,
				() => CurrentPage.As<ExercisePage>().IsNthExerciseIsSelected(indexOfSelectedEntry),
				"Expected exercise to be selected");

		}

		[When(@"I complete the first exercise")]
		public void WhenICompleteTheFirstExercise()
		{
			CurrentPage.As<ExercisePage>().ExecuteQueryAndWaitForResults(Settings.CurrentSettings.FirstExerciseQuery);
		}

		[Then(@"the complete button is displayed")]
		public void ThenTheCompleteButtonIsDisplayed()
		{
			CurrentPage.As<ExercisePage>().AssertCompleteButtonViaible();
		}

		[When(@"I press the Complete Button")]
		public void WhenIPressTheCompleteButton()
		{
			CurrentPage.As<ExercisePage>().ContinueButton.Click();
		}

	}
}
