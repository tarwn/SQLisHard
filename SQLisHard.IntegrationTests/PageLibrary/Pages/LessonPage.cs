
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.IntegrationTests.PageLibrary.Pages
{
	public class ExercisePage : PageBase
	{
		public override string DefaultTitle { get { return "SQL Is Hard - Exercise"; } }

		public override string PageUrl { get { return "/Exercise"; } }

		[FindsBy(How = How.Id, Using = "queryInput")]
		public IWebElement QueryInput { get; set; }

		[FindsBy(How = How.Id, Using = "queryExecutionButton")]
		public IWebElement QueryExecutionButton { get; set; }

		[FindsBy(How = How.Id, Using = "queryError")]
		public IWebElement QueryError { get; set; }

		[FindsBy(How = How.Id, Using = "queryResults")]
		public IWebElement QueryResults { get; set; }

		[FindsBy(How = How.CssSelector, Using = "#queryStatus")]
		public IWebElement QueryStatus { get; set; }

		[FindsBy(How = How.Id, Using = "dataTable")]
		public IWebElement DataTable { get; set; }

		[FindsBy(How = How.Id, Using = "moreResultsLink")]
		public IWebElement MoreResultsLink { get; set; }

		[FindsBy(How = How.Id, Using = "moreResultsLinkTotalCount")]
		public IWebElement MoreResultsLinkTotalCount { get; set; }

		public By ByResultRows { get { return By.CssSelector("#queryResults tbody tr"); } }

		public bool IsNotExecuting
		{
			get
			{
				return QueryExecutionButton.Enabled;
			}
		}

		public bool ResultsAreAvailableAndSuccessful
		{
			get
			{
				return QueryResults.IsPresent() && DataTable.IsPresent();
			}
		}

		public bool QueryErrorIsDisplayed
		{
			get
			{
				return QueryError.IsPresent();
			}
		}

		public bool MoreResultsLinkIsPresent
		{
			get
			{
				return MoreResultsLink.IsPresent();
			}
		}

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
			Assert(() => QueryStatus.Text.Trim().Equals(expectedStatus, StringComparison.InvariantCultureIgnoreCase), String.Format("Query status did not match expected value. Expected: '{0}', Actual: '{1}'", expectedStatus, QueryStatus.Text.Trim()));
		}
	}
}
