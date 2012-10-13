using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.IntegrationTests.PageLibrary
{
	public static class IWebElementExtensions
	{
		public static bool IsPresent(this IWebElement element)
		{
			try
			{
				bool b = element.Displayed;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static void ClearAndType(this IWebElement element, string value)
		{
			element.Clear();
			element.SendKeys(value);
		}
	}
}
