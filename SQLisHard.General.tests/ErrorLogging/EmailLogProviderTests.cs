using NUnit.Framework;
using SQLisHard.General.ErrorLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SQLisHard.General.tests.ErrorLogging
{
	[TestFixture]
	public class EmailLogProviderTests
	{
		[Test]
		public void GetHeaderHtml_NoValues_ReturnsEmptyString()
		{
			var provider = new EmailLogProvider("","","");

			var result = provider.GetHeaderHtml(new Dictionary<string, IEnumerable<string>>());

			Assert.AreEqual("", result);
		}

		[Test]
		public void GetHeaderHtml_SingleValuePerKey_ReturnsOneRowPerKey()
		{
			var provider = new EmailLogProvider("", "", "");
			var headers = new Dictionary<string, IEnumerable<string>>();
			headers.Add("a", new List<string>() { "1" });
			headers.Add("b", new List<string>() { "1" });
			headers.Add("c", new List<string>() { "1" });

			var result = provider.GetHeaderHtml(headers);

			int rowCount = Regex.Matches(result, "<tr>").Count;
			Assert.AreEqual(3, rowCount);
		}

		[Test]
		public void GetHeaderHtml_MultipleValuesPerKey_ReturnsOneRowPerKey()
		{
			var provider = new EmailLogProvider("", "", "");
			var headers = new Dictionary<string, IEnumerable<string>>();
			headers.Add("a", new List<string>() { "1", "2", "3" });
			headers.Add("b", new List<string>() { "1", "2" });
			headers.Add("c", new List<string>() { "1" });

			var result = provider.GetHeaderHtml(headers);

			int rowCount = Regex.Matches(result, "<tr>").Count;
			Assert.AreEqual(3, rowCount);
		}

		[Test]
		public void GetHeaderHtml_MultipleValuesPerKey_ReturnsAllValues()
		{
			var provider = new EmailLogProvider("", "", "");
			var headers = new Dictionary<string, IEnumerable<string>>();
			headers.Add("a", new List<string>() { "1", "2", "3" });
			headers.Add("b", new List<string>() { "1", "2" });
			headers.Add("c", new List<string>() { "1" });

			var result = provider.GetHeaderHtml(headers);

			Assert.IsTrue(Regex.IsMatch(result, @"a.*1.*2.*3.*\n?.*b.*1.*2.*\n?.*c.*1", RegexOptions.Multiline));
		}

		[Test]
		public void GetExceptionHtml_SingleException_ReturnsMessageContainingException()
		{
			string outerExceptionText = "unittest-exc-1";
			var exc = new Exception(outerExceptionText);
			var provider = new EmailLogProvider("", "", "");

			var result = provider.GetExceptionHtml(exc);

			Assert.IsTrue(result.Contains(outerExceptionText));
		}

		[Test]
		public void GetExceptionHtml_ExceptionWithInnerException_ReturnsMessageContainingInnerException()
		{
			string outerExceptionText = "unittest-exc-1";
			string innerExceptionText = "unittest-exc-1-i";
			var exc = new Exception(outerExceptionText, new Exception(innerExceptionText));
			var provider = new EmailLogProvider("", "", "");

			var result = provider.GetExceptionHtml(exc);

			Assert.IsTrue(result.Contains(innerExceptionText));
		}

		[Test]
		public void GetExceptionHtml_AggregateException_ReturnsMessageContainingInnerException()
		{
			string outerExceptionText = "unittest-exc-1";
			string innerExceptionText = "unittest-exc-1-i";
			var exc = new AggregateException(outerExceptionText, new Exception(innerExceptionText));
			var provider = new EmailLogProvider("", "", "");

			var result = provider.GetExceptionHtml(exc);

			Assert.IsTrue(result.Contains(innerExceptionText));
		}

		[Test]
		public void GetExceptionHtml_AggregateExceptionMultipleExceptions_ReturnsMessageContainingAllInnerExceptions()
		{
			string outerExceptionText = "unittest-exc-1";
			var aggregateTexts = new List<string>() { "agg-1", "agg-2", "agg-3" };
			var innerExceptions = aggregateTexts.Select(s => new Exception(s));
			var exc = new AggregateException(outerExceptionText, innerExceptions);
			var provider = new EmailLogProvider("", "", "");

			var result = provider.GetExceptionHtml(exc);

			foreach (string s in aggregateTexts)
				Assert.IsTrue(result.Contains(s), String.Format("Result did not contain '{0}'", s));
		}

		[Test]
		public void BuildMessage_ValidArguments_MessageContainsRequestUri()
		{
			var logArgs = new LogArguments(){ RequestURI = "REQUEST_URI" };
			var provider = new EmailLogProvider("", "", "");

			var result = provider.BuildMessage(new Exception("unittest-exc-1"), logArgs);

			Assert.IsTrue(result.Contains(logArgs.RequestURI));
		}

		[Test]
		public void BuildMessage_ValidArguments_MessageContainsUserId()
		{
			var logArgs = new LogArguments() { UserId = 1234567890 };
			var provider = new EmailLogProvider("", "", "");

			var result = provider.BuildMessage(new Exception("unittest-exc-1"), logArgs);

			Assert.IsTrue(result.Contains(logArgs.UserId.ToString()));
		}

		[Test]
		public void BuildMessage_ValidArguments_MessageContainsUsername()
		{
			var logArgs = new LogArguments() { Username = "USERNAMEVALUE" };
			var provider = new EmailLogProvider("", "", "");

			var result = provider.BuildMessage(new Exception("unittest-exc-1"), logArgs);

			Assert.IsTrue(result.Contains(logArgs.Username));
		}

		[Test]
		public void BuildMessage_ValidArguments_MessageContainsException()
		{
			var logArgs = new LogArguments() { };
			var provider = new EmailLogProvider("", "", "");

			var result = provider.BuildMessage(new Exception("unittest-exc-1"), logArgs);

			Assert.IsTrue(result.Contains("unittest-exc-1"));
		}

		[Test]
		public void BuildMessage_ValidArguments_MessageContainsHeaders()
		{
			var logArgs = new LogArguments() {
				Headers = new Dictionary<string, IEnumerable<string>>() { 
					{ "HEADERKEY", new List<string>(){ "VALUE" }}
				}
			};
			var provider = new EmailLogProvider("", "", "");

			var result = provider.BuildMessage(new Exception("unittest-exc-1"), logArgs);

			Assert.IsTrue(result.Contains("HEADERKEY"));
		}

	}
}
;