using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Web;

namespace SQLisHard.General.ErrorLogging
{
	public class EmailErrorReporter : IErrorReporter
	{
        private IFluentEmailFactory _emailFactory;
        private IOptions<EmailErrorSettings> _settings;
        private IWebHostEnvironment _environment;
        private ILogger<EmailErrorReporter> _logger;

        public EmailErrorReporter(IFluentEmailFactory emailFactory, IOptions<EmailErrorSettings> settings, IWebHostEnvironment environment, ILogger<EmailErrorReporter> logger)
        {
            _emailFactory = emailFactory;
			_settings = settings;
			_environment = environment;
            _logger = logger;
        }

		public void LogException(Exception exception, LogArguments logArguments)
		{
			var env = _environment.EnvironmentName;
			string message = BuildMessage(exception, logArguments);
			SendEmail("AUTOMATED MESSAGE - " + env + ": Unhandled " + exception.GetType().FullName, message);
		}

		private void SendEmail(string subject, string message)
		{
			var email = _emailFactory.Create()
				.To(_settings.Value.toAddress)
				.Subject(subject)
				.Body(message, true);

			var result = email.Send();

            if (!result.Successful)
            {
                _logger.LogError("Failed to send an email.\n{Errors}", string.Join(Environment.NewLine, result.ErrorMessages));
            }
		}

		public string BuildMessage(Exception exception, LogArguments logArguments)
		{
			string headers = GetHeaderHtml(logArguments.Headers);
			string exceptionDetails = GetExceptionHtml(exception);

			var message = string.Format(@"
			<style>
				th{{ background-color: #dddddd; min-width: 125px; padding: 0px 3px;}}
				td{{ padding: 0px 3px; border-bottom: 1px solid #eeeeee; }}
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
			</table>",
					 DateTime.UtcNow,
					 HttpUtility.HtmlEncode(logArguments.RequestURI),
					 logArguments.UserId,
					 HttpUtility.HtmlEncode(logArguments.Username),
					 exceptionDetails,
					 headers);

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
											 HttpUtility.HtmlEncode(exception.GetType().FullName),
											 HttpUtility.HtmlEncode(exception.Message),
											 HttpUtility.HtmlEncode(exception.ToString()),
											 HttpUtility.HtmlEncode(exception.StackTrace),
											 innerExceptions);
		}

		public string GetHeaderHtml(Dictionary<string, string> headers)
		{
			return String.Join("\n", headers.Select(kvp => String.Format("<tr><th align='right' valign='top'>{0}:</th><td>{1}</td></tr>", 
				HttpUtility.HtmlEncode(kvp.Key), 
				HttpUtility.HtmlEncode(kvp.Value))));
		}

	}
}
