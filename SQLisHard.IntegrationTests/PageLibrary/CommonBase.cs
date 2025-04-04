using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using NUnit.Framework;
using System.Threading;

namespace SQLisHard.IntegrationTests.PageLibrary
{
	public abstract class CommonBase
	{
		public IWebDriver Driver { get; set; }

		public void WaitUpTo(int milliseconds, Func<bool> Condition, string description)
		{
			int timeElapsed = 0;
			while (!Condition() && timeElapsed < milliseconds)
			{
				Thread.Sleep(100);
				timeElapsed += 100;
			}

			if (timeElapsed >= milliseconds || !Condition())
			{
				throw new AssertionException("Timed out while waiting for: " + description);
			}
		}

		public static void AssertIsEqual(string expectedValue, string actualValue, string elementDescription)
		{
			if (expectedValue != actualValue)
			{
				throw new AssertionException($"AssertIsEqual Failed: '{elementDescription}' didn't match expectations. Expected [{expectedValue}], Actual [{actualValue}]");
			}
		}

		public static void Assert(Func<bool> methodToEvaluate, string description)
		{
			if (methodToEvaluate() == false)
			{
				throw new AssertionException($"Assert Failed: {description}");
			}
		}

		public static void AssertIsEqual(int expectedValue, int actualValue, string elementDescription)
		{
			if (expectedValue != actualValue)
			{
				throw new AssertionException($"AssertIsEqual Failed: '{elementDescription}' didn't match expectations. Expected [{expectedValue}], Actual [{actualValue}]");
			}
		}

		public static void AssertElementPresent(IWebElement element, string elementDescription)
		{
			if (!element.IsPresent())
				throw new AssertionException($"AssertElementPresent Failed: Could not find '{elementDescription}'");
		}

		public static bool IsElementPresent(ISearchContext context, By searchBy)
		{
			try
			{
				bool b = context.FindElement(searchBy).Displayed;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static void AssertElementPresent(ISearchContext context, By searchBy, string elementDescription)
		{
			if (!IsElementPresent(context, searchBy))
				throw new AssertionException($"AssertElementPresent Failed: Could not find '{elementDescription}'");
		}

		public static void AssertElementsPresent(IWebElement[] elements, string elementDescription)
		{
			if (elements.Length == 0)
				throw new AssertionException($"AssertElementsPresent Failed: Could not find '{elementDescription}'");
		}

		public static void AssertElementText(IWebElement element, string expectedValue, string elementDescription)
		{
			AssertElementPresent(element, elementDescription);
			string actualtext = element.Text;
			if (actualtext != expectedValue)
			{
				throw new AssertionException($"AssertElementText Failed: Value for '{elementDescription}' did not match expectations. Expected: [{expectedValue}], Actual: [{actualtext}]");
			}
		}
	}
}
