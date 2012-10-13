
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.IntegrationTests.PageLibrary.Pages
{
	public class LessonPage : PageBase
	{
		public override string DefaultTitle { get { return "SQL Is Hard - Lesson"; } }

		public override string PageUrl { get { return "/Lesson"; } }

		[FindsBy(How = How.Id, Using = "QueryEditor")]
		public IWebElement QueryEditorInput { get; set; }

		[FindsBy(How = How.Id, Using = "QueryExecutionButton")]
		public IWebElement QueryExecutionButton { get; set; }

		[FindsBy(How = How.Id, Using = "QueryError")]
		public IWebElement QueryErrorArea { get; set; }

		[FindsBy(How = How.Id, Using = "QueryResults")]
		public IWebElement QueryResultsArea { get; set; }

		[FindsBy(How = How.Id, Using = "DataTable")]
		public IWebElement DataTable { get; set; }

		[FindsBy(How = How.Id, Using = "MoreResultsLink")]
		public IWebElement MoreResultsLink { get; set; }

		[FindsBy(How = How.Id, Using = "MoreResultsLinkTotalCount")]
		public IWebElement MoreResultsLinkTotalCount { get; set; }

		public By ByResultRows { get { return By.CssSelector("#QueryResults tbody tr"); } }

		public bool ResultsAreAvailableAndSuccessful
		{
			get
			{
				return QueryResultsArea.IsPresent() && DataTable.IsPresent();
			}
		}

		public bool QueryErrorIsDisplayed
		{
			get
			{
				return QueryErrorArea.IsPresent();
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
			QueryEditorInput.ClearAndType(queryText);
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
	}
}
