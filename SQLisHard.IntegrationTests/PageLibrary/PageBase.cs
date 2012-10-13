using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.PageObjects;
using NUnit.Framework;

namespace SQLisHard.IntegrationTests.PageLibrary
{
	public abstract partial class PageBase : CommonBase {
		public abstract string DefaultTitle { get; }
		public abstract string PageUrl { get; }

		public string BaseURL { get; set; }

		public static TPage LoadPage<TPage>(RemoteWebDriver driver, string baseUrl) where TPage : PageBase, new()
		{
			var url = baseUrl.TrimEnd(new char[] { '/' }) + (new TPage()).PageUrl;
			driver.Navigate().GoToUrl(url);
			return GetInstance<TPage>(driver, baseUrl, "");
		}

		protected TPage GetInstance<TPage>(RemoteWebDriver driver = null, string expectedTitle = "") where TPage : PageBase, new() {
			return GetInstance<TPage>(driver ?? Driver, BaseURL, expectedTitle);
		}

		protected static TPage GetInstance<TPage>(RemoteWebDriver driver, string baseUrl, string expectedTitle = "") where TPage : PageBase, new() {
			TPage pageInstance = new TPage() {
				Driver = driver,
				BaseURL = baseUrl
			};
			PageFactory.InitElements(driver, pageInstance);

			if (string.IsNullOrWhiteSpace(expectedTitle)) expectedTitle = pageInstance.DefaultTitle;

			//wait up to 5s for an actual page title since Selenium no longer waits for page to load after 2.21
			new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(5))
											.Until<OpenQA.Selenium.IWebElement>((d) => {
												return d.FindElement(ByChained.TagName("body"));
											});
			
			AssertIsEqual(expectedTitle, driver.Title, "Page Title");

			return pageInstance;
		}

		/// <summary>
		/// Asserts that the current page is of the given type
		/// </summary>
		public void Is<TPage>() where TPage : PageBase, new() {
			if (!(this is TPage)) {
				throw new AssertionException(String.Format("Page Type Mismatch: Current page is not a '{0}'", typeof(TPage).Name));
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
