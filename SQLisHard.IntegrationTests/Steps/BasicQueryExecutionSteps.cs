using SQLisHard.IntegrationTests.Configs;
using SQLisHard.IntegrationTests.PageLibrary;
using SQLisHard.IntegrationTests.PageLibrary.Pages;
using System;
using TechTalk.SpecFlow;

namespace SQLisHard.IntegrationTests.Steps
{
    [Binding]
    public class BasicQueryExecutionSteps : StepsBase
    {
        [Given(@"I am on the Exercise Page")]
        public void GivenIAmOnTheExercisePage()
        {
			CurrentPage = PageBase.LoadPage<ExercisePage>(CurrentDriver, Settings.CurrentSettings.BaseUrl);
			CurrentPage.As<ExercisePage>().WaitUpTo(5000, () => CurrentPage.As<ExercisePage>().IsNthExerciseIsSelected(1), "First exercise is loaded");
        }
        
        [Given(@"I have entered a query of ""(.*)""")]
        public void GivenIHaveEnteredAQueryOf(string queryText)
        {
			CurrentPage.As<ExercisePage>().EnterQuery(queryText);
        }

		[Given(@"I Press Execute")]
        [When(@"I Press Execute")]
        public void WhenIPressExecute()
        {
			CurrentPage.As<ExercisePage>().QueryExecutionButton.Click();
		}
        
        [Then(@"the query results are displayed")]
        public void ThenTheQueryResultsAreDisplayed()
        {
			CurrentPage.As<ExercisePage>().WaitUpTo(5000, () => CurrentPage.As<ExercisePage>().ResultsAreAvailableAndSuccessful, "Results to be displayed");
        }

		[Then(@"the query reports an error")]
		public void ThenTheQueryReportsAnError()
		{
			CurrentPage.As<ExercisePage>().WaitUpTo(5000, () => CurrentPage.As<ExercisePage>().QueryErrorIsDisplayed, "Query error to be displayed");
		}

		[Given(@"(.*) query results are displayed with a link to read more")]
		[Then(@"(.*) query results are displayed with a link to read more")]
		public void ThenQueryResultsAreDisplayedWithALinkToReadMore(int expectedResultCount)
		{
			CurrentPage.As<ExercisePage>().WaitUpTo(5000, () => CurrentPage.As<ExercisePage>().MoreResultsLinkIsPresent, "More results link to be displayed");
			CurrentPage.As<ExercisePage>().AssertMoreResultsLinkReportsTotalGreaterThan(expectedResultCount);
		}

		[Then(@"(.*) result rows are displayed")]
		public void ThenResultRowsAreDisplayed(int expectedResultCount)
		{
			CurrentPage.As<ExercisePage>().WaitUpTo(5000, () => CurrentPage.As<ExercisePage>().ResultsAreAvailableAndSuccessful, "Result rows to be displayed");
			CurrentPage.As<ExercisePage>().AssertNumberOfResultsRowsIs(expectedResultCount);
		}

		[Then(@"(.*) result rows are displayed without the read more link")]
		public void ThenResultRowsAreDisplayedWithoutTheReadMoreLink(int expectedResultCount)
		{
			CurrentPage.As<ExercisePage>().WaitUpTo(5000, () => !CurrentPage.As<ExercisePage>().MoreResultsLinkIsPresent, "More results link to disappear");
			ThenResultRowsAreDisplayed(expectedResultCount);
		}

		[When(@"I click the read more link")]
		public void WhenIClickTheReadMoreLink()
		{
			CurrentPage.As<ExercisePage>().MoreResultsLink.Click();
		}

		[Then(@"the query status displays ""(.*)""")]
		public void ThenTheQueryStatusDisplays(string expectedStatus)
		{
			CurrentPage.As<ExercisePage>().WaitUpTo(5000, () => CurrentPage.As<ExercisePage>().IsNotExecuting, "Page to stop executing");
			CurrentPage.As<ExercisePage>().AssertStatusDisplays(expectedStatus);
		}

		[Given(@"I have selected a pattern-based exercise")]
		public void GivenIHaveSelectedAPattern_BasedExercise()
		{
			CurrentPage.As<ExercisePage>().SelectExercise(Settings.CurrentSettings.PatternExerciseId);
			CurrentPage.As<ExercisePage>().WaitUpTo(5000, () => CurrentPage.As<ExercisePage>().IsCurrentExercise(Settings.CurrentSettings.PatternExerciseId), "Exercise has not updated");
		}

		[Then(@"a tip is shown for the pattern")]
		public void ThenATipIsShownForThePattern()
		{
			CurrentPage.As<ExercisePage>().WaitUpTo(5000, () => CurrentPage.As<ExercisePage>().TipTabIsVisible, "Tip description is not visible");
		}

    }
}
