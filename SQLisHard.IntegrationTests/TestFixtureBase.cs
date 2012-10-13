using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Firefox;
using TechTalk.SpecFlow;
using OpenQA.Selenium;
using System.Drawing.Imaging;
using SQLisHard.IntegrationTests.Configs;

namespace SQLisHard.IntegrationTests {
	public class TestFixtureBase {
		protected RemoteWebDriver CurrentDriver { get; set; }

		[SetUp]
		public void Test_Setup() {
			FirefoxBinary fb;
			if (!String.IsNullOrWhiteSpace(Settings.CurrentSettings.FirefoxBinaryPath)) {
				fb = new FirefoxBinary(Settings.CurrentSettings.FirefoxBinaryPath);
			}
			else {
				fb = new FirefoxBinary();
			}
			CurrentDriver = new FirefoxDriver(fb, new FirefoxProfile());
		}

		

		[TearDown]
		public void Test_Teardown() {
			try {
				if (TestContext.CurrentContext.Result.Status == TestStatus.Failed
						&& CurrentDriver is ITakesScreenshot) {
					((ITakesScreenshot)CurrentDriver).GetScreenshot().SaveAsFile(TestContext.CurrentContext.Test.FullName + ".jpg", ImageFormat.Jpeg);
				}
			}
			catch { }	// null ref exception occurs from accessing TestContext.CurrentContext.Result.Status property

			try {
				CurrentDriver.Quit();
			}
			catch { }
		}

	}

}
