using SQLisHard.Core;
using SQLisHard.General.ErrorLogging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SQLisHard.Attributes.MVC
{
	public class CaptureUnhandledExceptionAttribute : FilterAttribute, IExceptionFilter
	{
		private ILogProvider _logProvider;

		public CaptureUnhandledExceptionAttribute()
			: this(new EmailLogProvider(
				ConfigurationManager.AppSettings["Environment.Name"],
				"sqlishard@outlook.com",	//TODO: come back and clean this up
				"sqlishard@outlook.com"		//TODO: come back and clean this up
				)) { }

		public CaptureUnhandledExceptionAttribute(ILogProvider logProvider)
		{
			_logProvider = logProvider;
		}

		public void OnException(ExceptionContext context)
		{
			context.ExceptionHandled = false;

			var args = new LogArguments(context.HttpContext.Request) {
				UserId = 0,
				Username = "(Not Auth'd)"
			};
			if (context.HttpContext.User.Identity.IsAuthenticated)
			{
				var user = context.HttpContext.User.Identity as UserPrincipal;
				if (user != null)
				{
					args.UserId = user.UserIdentity.Id.Value;
					args.Username = user.UserIdentity.Name;
				}
				else
				{
					args.Username = context.HttpContext.User.Identity.Name + " (not verified)";
				}
			}

			_logProvider.LogException(context.Exception, args);
		}
	}
}