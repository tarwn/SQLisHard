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
using NUnit.Framework.Interfaces;
using OpenQA.Selenium.Support.Extensions;

namespace SQLisHard.IntegrationTests
{
    [TestFixture]
    public class TestFixtureBase
    {
        protected IWebDriver CurrentDriver { get; set; } = null!;

        [SetUp]
        public void Test_Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("disable-gpu");
            options.AddArguments("window-size=1280,1024");
            CurrentDriver = new ChromeDriver(options);
            CurrentDriver.Manage().Window.Size = new System.Drawing.Size(1280, 1024);
        }

        [TearDown]
        public void Test_Teardown()
        {
            try
            {
                if (CurrentDriver is ITakesScreenshot && TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                {
                    ((ITakesScreenshot)CurrentDriver).GetScreenshot().SaveAsFile(TestContext.CurrentContext.Test.FullName + ".png");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception capturing screenshot: " + exc.Message);
            }

            try
            {
                CurrentDriver.Quit();
            }
            catch { }
        }
    }
}

