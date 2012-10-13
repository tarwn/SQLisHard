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
        [Given(@"I am on the Lesson Page")]
        public void GivenIAmOnTheLessonPage()
        {
			CurrentPage = PageBase.LoadPage<LessonPage>(CurrentDriver, Settings.CurrentSettings.BaseUrl);
        }
        
        [Given(@"I have entered a query of ""(.*)""")]
        public void GivenIHaveEnteredAQueryOf(string queryText)
        {
			CurrentPage.As<LessonPage>().EnterQuery(queryText);
        }

		[Given(@"I Press Execute")]
        [When(@"I Press Execute")]
        public void WhenIPressExecute()
        {
			CurrentPage.As<LessonPage>().QueryExecutionButton.Click();
		}
        
        [Then(@"the query results are displayed")]
        public void ThenTheQueryResultsAreDisplayed()
        {
			CurrentPage.As<LessonPage>().WaitUpTo(5000, () => CurrentPage.As<LessonPage>().ResultsAreAvailableAndSuccessful, "Results to be displayed");
        }

		[Then(@"the query reports an error")]
		public void ThenTheQueryReportsAnError()
		{
			CurrentPage.As<LessonPage>().WaitUpTo(5000, () => CurrentPage.As<LessonPage>().QueryErrorIsDisplayed, "Query error to be displayed");
		}

		[Given(@"(.*) query results are displayed with a link to read more")]
		[Then(@"(.*) query results are displayed with a link to read more")]
		public void ThenQueryResultsAreDisplayedWithALinkToReadMore(int expectedResultCount)
		{
			CurrentPage.As<LessonPage>().WaitUpTo(5000, () => CurrentPage.As<LessonPage>().MoreResultsLinkIsPresent, "More results link to be displayed");
			CurrentPage.As<LessonPage>().AssertMoreResultsLinkReportsTotalGreaterThan(expectedResultCount);
		}

		[Then(@"(.*) result rows are displayed")]
		public void ThenResultRowsAreDisplayed(int expectedResultCount)
		{
			CurrentPage.As<LessonPage>().WaitUpTo(5000, () => CurrentPage.As<LessonPage>().ResultsAreAvailableAndSuccessful, "Result rows to be displayed");
			CurrentPage.As<LessonPage>().AssertNumberOfResultsRowsIs(expectedResultCount);
		}

		[When(@"I click the read more link")]
		public void WhenIClickTheReadMoreLink()
		{
			CurrentPage.As<LessonPage>().MoreResultsLink.Click();
		}

    }
}
