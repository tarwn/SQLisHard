using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SQLisHard.IntegrationTests.PageLibrary
{
	public abstract partial class PageBase : CommonBase {
		public abstract string DefaultTitle { get; }
		public abstract string PageUrl { get; }

		public string BaseURL { get; set; } = string.Empty;

		public static TPage LoadPage<TPage>(IWebDriver driver, string baseUrl) where TPage : PageBase, new()
		{
			var url = baseUrl.TrimEnd('/') + (new TPage()).PageUrl;
			driver.Navigate().GoToUrl(url);
			return GetInstance<TPage>(driver, baseUrl, "");
		}

		protected TPage GetInstance<TPage>(IWebDriver? driver = null, string expectedTitle = "") where TPage : PageBase, new() {
			return GetInstance<TPage>(driver ?? Driver, BaseURL, expectedTitle);
		}

		protected static TPage GetInstance<TPage>(IWebDriver driver, string baseUrl, string expectedTitle = "") where TPage : PageBase, new() {
			TPage pageInstance = new TPage() {
				Driver = driver,
				BaseURL = baseUrl
			};

			if (string.IsNullOrWhiteSpace(expectedTitle)) expectedTitle = pageInstance.DefaultTitle;

			//wait up to 5s for an actual page title since Selenium no longer waits for page to load after 2.21
			new WebDriverWait(driver, TimeSpan.FromSeconds(5))
				.Until(d => d.FindElement(By.TagName("body")));
			
			AssertIsEqual(expectedTitle, driver.Title, "Page Title");

			return pageInstance;
		}

		/// <summary>
		/// Asserts that the current page is of the given type
		/// </summary>
		public void Is<TPage>() where TPage : PageBase, new() {
			if (!(this is TPage)) {
				throw new AssertionException($"Page Type Mismatch: Current page is not a '{typeof(TPage).Name}'");
			}
		}

		/// <summary>
		/// Inline cast to the given page type
		/// </summary>
		public TPage As<TPage>() where TPage : PageBase, new() {
			return (TPage)this;
		}
	}

}
