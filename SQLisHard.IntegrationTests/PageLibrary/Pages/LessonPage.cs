using OpenQA.Selenium;

namespace SQLisHard.IntegrationTests.PageLibrary.Pages
{
	public class ExercisePage : PageBase
	{
		public override string DefaultTitle => "SQL Is Hard - Exercise";
		public override string PageUrl => "/Exercise";

		public IWebElement ContinueButton => Driver.FindElement(By.Id("continueButton"));
		public IWebElement QueryInput => Driver.FindElement(By.Id("queryInput"));
		public IWebElement QueryExecutionButton => Driver.FindElement(By.Id("queryExecutionButton"));
		public IWebElement QueryError => Driver.FindElement(By.Id("queryError"));
		public IWebElement TipDescription => Driver.FindElement(By.Id("tipDescription"));
		public IWebElement QueryResults => Driver.FindElement(By.Id("queryResults"));
		public IWebElement QueryStatus => Driver.FindElement(By.CssSelector("#queryStatus"));
		public IWebElement DataTable => Driver.FindElement(By.Id("dataTable"));
		public IWebElement MoreResultsLink => Driver.FindElement(By.Id("moreResultsLink"));
		public IWebElement MoreResultsLinkTotalCount => Driver.FindElement(By.Id("moreResultsLinkTotalCount"));
		public IWebElement ExerciseTitle => Driver.FindElement(By.Id("exerciseTitle"));

		private By ByResultRows => By.CssSelector("#queryResults tbody tr");
		private By ByExerciseListItems => By.CssSelector("#exerciseList ul li");

		public bool IsNotExecuting => QueryExecutionButton.Enabled;

		public bool ResultsAreAvailableAndSuccessful => QueryResults.IsPresent() && DataTable.IsPresent();

		public bool QueryErrorIsDisplayed => QueryError.IsPresent();

		public bool TipTabIsVisible => TipDescription.IsPresent();

		public bool MoreResultsLinkIsPresent => MoreResultsLink.IsPresent();

		public void EnterQuery(string queryText)
		{
			QueryInput.ClearAndType(queryText);
		}

		public void AssertMoreResultsLinkReportsTotalOf(int expectedResultCount)
		{
			AssertElementText(MoreResultsLinkTotalCount, expectedResultCount.ToString(), "Total count in the More Results link");
		}
		
		public void AssertNumberOfResultsRowsIs(int expectedResultCount)
		{
			AssertIsEqual(expectedResultCount, Driver.FindElements(ByResultRows).Count, "Expected rows count doesn't match actual count");
		}

		public void AssertMoreResultsLinkReportsTotalGreaterThan(int expectedResultCount)
		{
			Assert(() => int.Parse(MoreResultsLinkTotalCount.Text) > expectedResultCount, "Total count in the More Results link");
		}

		public void AssertStatusDisplays(string expectedStatus)
		{
			Assert(() => QueryStatus.Text.Trim().Equals(expectedStatus, StringComparison.InvariantCultureIgnoreCase), 
				$"Query status did not match expected value. Expected: '{expectedStatus}', Actual: '{QueryStatus.Text.Trim()}'");
		}

		internal void AssertNthExerciseIsSelected(int indexOfSelectedEntry)
		{
			var i = indexOfSelectedEntry - 1;
			var exerciseList = Driver.FindElements(ByExerciseListItems);
			Assert(() => exerciseList.Count > i, $"List is too short ({exerciseList.Count} items) to contain the #{indexOfSelectedEntry}");
			Assert(() => exerciseList[i].GetAttribute("class") == "selected",
				$"The indexed item ({exerciseList[i].Text}) is not selected (class: {exerciseList[i].GetAttribute("class")})");
		}

		internal void ExecuteQueryAndWaitForResults(string queryText)
		{
			EnterQuery(queryText);
			QueryExecutionButton.Click();
			WaitUpTo(5000, () => ResultsAreAvailableAndSuccessful, "Results to be displayed");
		}

		internal void AssertCompleteButtonViaible()
		{
			AssertElementPresent(ContinueButton, "ContinueButton");
		}

		internal bool IsNthExerciseIsSelected(int indexOfSelectedEntry)
		{
			var i = indexOfSelectedEntry - 1;
			var exerciseList = Driver.FindElements(ByExerciseListItems);
			return exerciseList.Count > i && exerciseList[i].GetAttribute("class") == "selected";
		}

		public bool ExercisesHaveLoaded()
		{ 
			var exerciseList = Driver.FindElements(ByExerciseListItems);
			return exerciseList.Count > 1;	// starts with a 'Loading...' item
		}

		public void SelectExercise(string exerciseId)
		{
			var exerciseLink = Driver.FindElement(By.CssSelector($"li[data-exerciseid='{exerciseId}']"));
			AssertElementPresent(exerciseLink, $"Exercise Link for '{exerciseId}'");
			exerciseLink.Click();
		}

		public bool IsCurrentExercise(string exerciseId)
		{
			AssertElementPresent(ExerciseTitle, "Exercise Title");
			return ExerciseTitle.GetAttribute("data-exerciseid") == exerciseId;
		}
	}
}
