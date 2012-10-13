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
    }
}
