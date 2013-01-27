using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace SQLisHard.General.ErrorLogging
{
	public class EmailLogProvider : ILogProvider
	{
		private string _fromAddress;
		private string _toAddress;
		private string _environment;

		public EmailLogProvider(string environmentName, string fromAddress, string toAddress)
		{
			_environment = environmentName;
			_fromAddress = fromAddress;
			_toAddress = toAddress;
		}

		public void LogException(Exception exception, LogArguments logArguments)
		{
			string message = BuildMessage(exception, logArguments);
			SendEmail("AUTOMATED MESSAGE - " + _environment.ToUpper() + ": Unhandled " + exception.GetType().FullName, message);
		}

		private void SendEmail(string subject, string message)
		{
			// uses configs from web.config
			var client = new SmtpClient();
			var mail = new MailMessage(_fromAddress, _toAddress);
			mail.Subject = subject;
			mail.Body = message;
			mail.IsBodyHtml = true;
			client.Send(mail);
		}

		public string BuildMessage(Exception exception, LogArguments logArguments)
		{
			string headers = GetHeaderHtml(logArguments.Headers);
			string serverVariables = GetServerVariablesHtml(logArguments.ServerVariables);

			string exceptionDetails = GetExceptionHtml(exception);

			var message = (string.Format(@"
			<style>
				th{{ background-color: #dddddd; min-width: 80px; padding: 0px 3px;}}
				.faketh{{ background-color: #dddddd; font-weight: bold; text-align: center; }}
				.headers td{{ max-width: 600px; }}
			</style>
			<table cellpadding='0' cellspacing='0'>
				<tr><td colspan='2' class='faketh'>General Info</td></tr>
				<tr><th align='right' valign='top'>Time:</th><td>{0}</td></tr>
				<tr><th align='right' valign='top'>URI:</th><td>{1}</td></tr>
				<tr><th align='right' valign='top'>User:</th><td>#{2}, {3}</td></tr>
			</table>
			<br />

			<table cellpadding='0' cellspacing='0'>
				<tr><td class='faketh'>Exception Details</td></tr>
				<tr><td>{4}</td></tr>
			</table>
			<br />

			<table class='headers' cellpadding='0' cellspacing='0'>
				<tr><td colspan='2' class='faketh'>HTTP Headers</td></tr>
				{5}
			</table>
			<br />

			<table class='headers' cellpadding='0' cellspacing='0'>
				<tr><td colspan='2' class='faketh'>ServerVariables</td></tr>
				{6}
			</table>",
					 DateTime.UtcNow,
					 logArguments.RequestURI,
					 logArguments.UserId,
					 logArguments.Username,
					 exceptionDetails,
					 headers,
					 serverVariables));

			return message;
		}

		public string GetExceptionHtml(Exception exception)
		{
			string innerExceptions = "";
			if (exception is AggregateException)
			{ 
				if (exception.InnerException != null)
					innerExceptions = "<tr><th align='right'>Inner Exception(s):</th><td>" + GetExceptionHtml(exception.InnerException) + "</td></tr>";

				foreach(var aggExc in ((AggregateException)exception).InnerExceptions)
					innerExceptions += "<tr><th align='right'>Aggregated Exception:</th><td>" + GetExceptionHtml(aggExc) + "</td></tr>";
			}
			else
			{
				if (exception.InnerException != null)
					innerExceptions = "<tr><th align='right'>Inner Exception(s):</th><td>" + GetExceptionHtml(exception.InnerException) + "</td></tr>";

			}

			return String.Format(@"<table cellpadding='0' cellspacing='0'>
										<tr><th valign='top' align='right'>Type</th><td>{0}</td></tr>
										<tr><th valign='top' align='right'>Message:</th><td>{1}</td></tr>
										<tr><th valign='top' align='right'>ToString:</th><td><pre>{2}</pre></td></tr>
										<tr><th valign='top' align='right'>Stack Trace</th><td><pre>{3}</pre></td></tr>
										{4}
									</table>",
											 exception.GetType().FullName,
											 exception.Message,
											 exception.ToString(),
											 exception.StackTrace,
											 innerExceptions);
		}

		public string GetHeaderHtml(Dictionary<string, IEnumerable<string>> headers)
		{
			return String.Join("\n", headers.Select(kvp => String.Format("<tr><th align='right' valign='top'>{0}:</th><td>{1}</td></tr>", kvp.Key, String.Join("<br/>", kvp.Value.Select(v => HttpUtility.HtmlEncode(v))))));
		}

		public string GetServerVariablesHtml(Dictionary<string, string> values)
		{
			return String.Join("\n", values.Select(v => String.Format("<tr><th align='right' valign='top'>{0}:</th><td>{1}</td></tr>", v.Key, v.Value)));
		}
	}
}
