using OpenQA.Selenium;
using SQLisHard.IntegrationTests.PageLibrary;
using TechTalk.SpecFlow;

namespace SQLisHard.IntegrationTests
{
	public class StepsBase : TestFixtureBase {

		private readonly ScenarioContext _scenarioContext;

		public StepsBase(ScenarioContext scenarioContext) {
			_scenarioContext = scenarioContext;
		}

		#region Properties for Readability

		/// <summary>
		/// Sets the Current page to the specified value - provided to help readability
		/// </summary>
		protected PageBase NextPage { set => CurrentPage = value; }

		#endregion

		protected PageBase CurrentPage {
			get => (PageBase)_scenarioContext["CurrentPage"];
			set => _scenarioContext["CurrentPage"] = value;
		}

		[BeforeScenario("UI")]
		public void BeforeScenario() {
			if (!_scenarioContext.ContainsKey("CurrentDriver")) {
				Test_Setup();
				_scenarioContext.Add("CurrentDriver", CurrentDriver);
			}
			else {
				CurrentDriver = (IWebDriver)_scenarioContext["CurrentDriver"];
			}
		}

		[AfterScenario("UI")]
		public void AfterScenario() {
			if (_scenarioContext.ContainsKey("CurrentDriver")) {
				Test_Teardown();
				_scenarioContext.Remove("CurrentDriver");
			}
		}
	}

}
