using NUnit.Framework;
using SQLisHard.General.ErrorLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;

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

			Assert.That(result, Is.EqualTo(""));
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
			Assert.That(rowCount, Is.EqualTo(3));
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
			Assert.That(rowCount, Is.EqualTo(3));
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

			Assert.That(Regex.IsMatch(result, @"a.*1.*2.*3.*\n?.*b.*1.*2.*\n?.*c.*1", RegexOptions.Multiline), Is.True);
		}

		[Test]
		public void GetServerVariablesHtml_NoValues_ReturnsEmptyString()
		{
			var provider = new EmailLogProvider("", "", "");

			var result = provider.GetServerVariablesHtml(new Dictionary<string, string>());

			Assert.That(result, Is.EqualTo(""));
		}

		[Test]
		public void GetServerVariablesHtml_HasValues_ReturnsOneRowPerKey()
		{
			var provider = new EmailLogProvider("", "", "");

			var result = provider.GetServerVariablesHtml(new Dictionary<string, string>() { 
				{"a", "1"},
				{"b", "1"},
				{"c", "1"}
			});

			int rowCount = Regex.Matches(result, "<tr>").Count;
			Assert.That(rowCount, Is.EqualTo(3));
		}

		[Test]
		public void GetServerVariablesHtml_WithValue_FormatsCorrectly()
		{
			var provider = new EmailLogProvider("test", "from", "to");
			var result = provider.GetServerVariablesHtml(new Dictionary<string, string>() { { "KEY", "VALUE" } });
			Assert.That(result, Is.EqualTo("<tr><th align='right' valign='top'>KEY:</th><td>VALUE</td></tr>"));
		}

		[Test]
		public void GetHeaderHtml_WithOneValue_FormatsCorrectly()
		{
			var provider = new EmailLogProvider("test", "from", "to");
			var result = provider.GetHeaderHtml(new Dictionary<string, IEnumerable<string>>() { { "KEY", new string[] { "VALUE" } } });
			Assert.That(result, Is.EqualTo("<tr><th align='right' valign='top'>KEY:</th><td>VALUE</td></tr>"));
		}

		[Test]
		public void GetHeaderHtml_WithTwoValues_FormatsCorrectly()
		{
			var provider = new EmailLogProvider("test", "from", "to");
			var result = provider.GetHeaderHtml(new Dictionary<string, IEnumerable<string>>() { { "KEY", new string[] { "VALUE", "VALUE2" } } });
			Assert.That(result, Is.EqualTo("<tr><th align='right' valign='top'>KEY:</th><td>VALUE<br/>VALUE2</td></tr>"));
		}

		[Test]
		public void GetHeaderHtml_WithHtmlValue_EncodesCorrectly()
		{
			var provider = new EmailLogProvider("test", "from", "to");
			var result = provider.GetHeaderHtml(new Dictionary<string, IEnumerable<string>>() { { "KEY", new string[] { "<VALUE>" } } });
			Assert.That(result.Contains("&lt;VALUE&gt;"), Is.True);
		}

		[Test]
		public void GetExceptionHtml_SingleException_ReturnsMessageContainingException()
		{
			string outerExceptionText = "unittest-exc-1";
			var exc = new Exception(outerExceptionText);
			var provider = new EmailLogProvider("", "", "");

			var result = provider.GetExceptionHtml(exc);

			Assert.That(result.Contains(outerExceptionText), Is.True);
		}

		[Test]
		public void GetExceptionHtml_ExceptionWithInnerException_ReturnsMessageContainingInnerException()
		{
			string outerExceptionText = "unittest-exc-1";
			string innerExceptionText = "unittest-exc-1-i";
			var exc = new Exception(outerExceptionText, new Exception(innerExceptionText));
			var provider = new EmailLogProvider("", "", "");

			var result = provider.GetExceptionHtml(exc);

			Assert.That(result.Contains(innerExceptionText), Is.True);
		}

		[Test]
		public void GetExceptionHtml_AggregateException_ReturnsMessageContainingInnerException()
		{
			string outerExceptionText = "unittest-exc-1";
			string innerExceptionText = "unittest-exc-1-i";
			var exc = new AggregateException(outerExceptionText, new Exception(innerExceptionText));
			var provider = new EmailLogProvider("", "", "");

			var result = provider.GetExceptionHtml(exc);

			Assert.That(result.Contains(innerExceptionText), Is.True);
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
				Assert.That(result.Contains(s), Is.True, $"Result did not contain '{s}'");
		}

		[Test]
		public void BuildMessage_ValidArguments_MessageContainsRequestUri()
		{
			var logArgs = new LogArguments(){ RequestURI = "REQUEST_URI" };
			var provider = new EmailLogProvider("", "", "");

			var result = provider.BuildMessage(new Exception("unittest-exc-1"), logArgs);

			Assert.That(result.Contains(logArgs.RequestURI), Is.True);
		}

		[Test]
		public void BuildMessage_ValidArguments_MessageContainsUserId()
		{
			var logArgs = new LogArguments() { UserId = 1234567890 };
			var provider = new EmailLogProvider("", "", "");

			var result = provider.BuildMessage(new Exception("unittest-exc-1"), logArgs);

			Assert.That(result.Contains(logArgs.UserId.ToString()), Is.True);
		}

		[Test]
		public void BuildMessage_ValidArguments_MessageContainsUsername()
		{
			var logArgs = new LogArguments() { Username = "USERNAMEVALUE" };
			var provider = new EmailLogProvider("", "", "");

			var result = provider.BuildMessage(new Exception("unittest-exc-1"), logArgs);

			Assert.That(result.Contains(logArgs.Username), Is.True);
		}

		[Test]
		public void BuildMessage_ValidArguments_MessageContainsException()
		{
			var logArgs = new LogArguments() { };
			var provider = new EmailLogProvider("", "", "");

			var result = provider.BuildMessage(new Exception("unittest-exc-1"), logArgs);

			Assert.That(result.Contains("unittest-exc-1"), Is.True);
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

			Assert.That(result.Contains("HEADERKEY"), Is.True);
		}

		[Test]
		public void BuildMessage_BasicException_IncludesType()
		{
			var provider = new EmailLogProvider("test", "from", "to");
			var args = new LogArguments();
			args.RequestURI = "http://example.com";
			args.UserId = 123;
			args.Username = "TestUser";
			var exc = new Exception("Test Message");

			string message = provider.BuildMessage(exc, args);
			Assert.That(message.Contains("System.Exception"), Is.True);
		}

		[Test]
		public void BuildMessage_BasicException_IncludesMessage()
		{
			var provider = new EmailLogProvider("test", "from", "to");
			var args = new LogArguments();
			args.RequestURI = "http://example.com";
			args.UserId = 123;
			args.Username = "TestUser";
			var exc = new Exception("Test Message");

			string message = provider.BuildMessage(exc, args);
			Assert.That(message.Contains("Test Message"), Is.True);
		}

		[Test]
		public void BuildMessage_BasicException_IncludesStackTrace()
		{
			var provider = new EmailLogProvider("test", "from", "to");
			var args = new LogArguments();
			args.RequestURI = "http://example.com";
			args.UserId = 123;
			args.Username = "TestUser";
			var exc = new Exception("Test Message");

			string message = "";
			try { throw exc; } catch(Exception caughtExc) { message = provider.BuildMessage(caughtExc, args); }

			Assert.That(message.Contains("EmailLogProviderTests.cs"), Is.True);
		}

		[Test]
		public void BuildMessage_AggregateException_IncludesAggregatedDetails()
		{
			var provider = new EmailLogProvider("test", "from", "to");
			var args = new LogArguments();
			args.RequestURI = "http://example.com";
			args.UserId = 123;
			args.Username = "TestUser";
			var innerExc1 = new InvalidOperationException("Inner 1");
			var innerExc2 = new ArgumentException("Inner 2");
			var exc = new AggregateException("Aggregate Message", innerExc1, innerExc2);

			string message = provider.BuildMessage(exc, args);

			Assert.That(message.Contains("System.AggregateException"), Is.True);
			Assert.That(message.Contains("Aggregate Message"), Is.True);
			Assert.That(message.Contains("Aggregated Exception:"), Is.True);
			Assert.That(message.Contains("System.InvalidOperationException"), Is.True);
			Assert.That(message.Contains("Inner 1"), Is.True);
			Assert.That(message.Contains("System.ArgumentException"), Is.True);
			Assert.That(message.Contains("Inner 2"), Is.True);
		}

		[Test]
		public void BuildMessage_ExceptionWithInner_IncludesInnerDetails()
		{
			var provider = new EmailLogProvider("test", "from", "to");
			var args = new LogArguments();
			args.RequestURI = "http://example.com";
			args.UserId = 123;
			args.Username = "TestUser";
			var innerExc = new ArgumentException("Inner Message");
			var exc = new Exception("Outer Message", innerExc);

			string message = provider.BuildMessage(exc, args);

			Assert.That(message.Contains("System.Exception"), Is.True);
			Assert.That(message.Contains("Outer Message"), Is.True);
			Assert.That(message.Contains("Inner Exception(s):"), Is.True);
			Assert.That(message.Contains("System.ArgumentException"), Is.True);
			Assert.That(message.Contains("Inner Message"), Is.True);
		}

		// TODO: Add tests for SendEmail if possible (might require mocking SmtpClient or abstracting email sending)
	}
}