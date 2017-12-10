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
using OpenQA.Selenium.Chrome;

namespace SQLisHard.IntegrationTests
{
	public class TestFixtureBase
	{
		protected RemoteWebDriver CurrentDriver { get; set; }

		[SetUp]
		public void Test_Setup()
		{
            //FirefoxBinary fb;
            //if (!String.IsNullOrWhiteSpace(Settings.CurrentSettings.FirefoxBinaryPath))
            //{
            //	fb = new FirefoxBinary(Settings.CurrentSettings.FirefoxBinaryPath);
            //}
            //else
            //{
            //	fb = new FirefoxBinary();
            //}
            //CurrentDriver = new FirefoxDriver(fb, new FirefoxProfile());
            CurrentDriver = new ChromeDriver(Environment.CurrentDirectory);
		}

        [TearDown]
		public void Test_Teardown()
		{
			try
			{
				if (CurrentDriver is ITakesScreenshot && TestContext.CurrentContext.Result.Status == TestStatus.Failed)
				{
					((ITakesScreenshot)CurrentDriver).GetScreenshot().SaveAsFile(TestContext.CurrentContext.Test.FullName + ".jpg", ScreenshotImageFormat.Jpeg);
				}
			}
			catch (Exception exc)
			{
				Console.WriteLine("Exception capturing screenshot: " + exc.Message);
			}	// null ref exception occurs from accessing TestContext.CurrentContext.Result.Status property

			try
			{
				CurrentDriver.Quit();
			}
			catch { }
		}

	}

}
