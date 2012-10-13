using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using OpenQA.Selenium.Remote;
using SQLisHard.IntegrationTests.PageLibrary;

namespace SQLisHard.IntegrationTests
{
	public class StepsBase : TestFixtureBase {

		#region Properties for Readability

		/// <summary>
		/// Sets the Current page to the specified value - provided to help readability
		/// </summary>
		protected PageBase NextPage { set { CurrentPage = value; } }

		#endregion

		protected PageBase CurrentPage {
			get { return (PageBase)ScenarioContext.Current["CurrentPage"]; }
			set { ScenarioContext.Current["CurrentPage"] = value; }
		}

		[BeforeScenario("UI")]
		public void BeforeScenario() {
			if (!ScenarioContext.Current.ContainsKey("CurrentDriver")) {
				Test_Setup();
				ScenarioContext.Current.Add("CurrentDriver", CurrentDriver);
			}
			else {
				CurrentDriver = (RemoteWebDriver)ScenarioContext.Current["CurrentDriver"];
			}
		}

		[AfterScenario("UI")]
		public void AfterScenario() {
			if (ScenarioContext.Current.ContainsKey("CurrentDriver")) {
				Test_Teardown();
				ScenarioContext.Current.Remove("CurrentDriver");
			}
		}
	}

}
