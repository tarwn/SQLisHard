
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

		[FindsBy(How = How.Id, Using = "QueryResults")]
		public IWebElement QueryResultsArea { get; set; }

		[FindsBy(How = How.Id, Using = "DataTable")]
		public IWebElement DataTable { get; set; }

		public bool ResultsAreAvailableAndSuccessful
		{
			get
			{
				return QueryResultsArea.IsPresent() && DataTable.IsPresent();
			}
		}

		public void EnterQuery(string queryText)
		{
			QueryEditorInput.ClearAndType(queryText);
		}
	}
}
