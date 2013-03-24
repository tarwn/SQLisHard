using SQLisHard.Core;
using SQLisHard.General.ErrorLogging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace SQLisHard.Attributes.WebAPI
{
	public class CaptureUnhandledExceptionAttribute : ExceptionFilterAttribute
	{
		private ILogProvider _logProvider;
				
		public CaptureUnhandledExceptionAttribute() : this(new EmailLogProvider(
			ConfigurationManager.AppSettings["Environment.Name"],
			"sqlishard@outlook.com",	//TODO: come back and clean this up
			"sqlishard@outlook.com"		//TODO: come back and clean this up
			)) { }

		public CaptureUnhandledExceptionAttribute(ILogProvider logProvider)
		{
			_logProvider = logProvider;
		}

		public override bool AllowMultiple { get { return true; } }

		public override void OnException(HttpActionExecutedContext context)
		{
			context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

			var args = new LogArguments(context.Request) { 
				UserId = 0,
				Username = "(Not Auth'd)"
			};
			if(HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
			{
				var user = HttpContext.Current.User as UserPrincipal;
				if (user != null)
				{
					args.UserId = user.UserIdentity.Id.Value;
					args.Username = user.UserIdentity.Name;
				}
				else
				{
					args.Username = HttpContext.Current.User.Identity.Name + " (not verified)";
				}
			}

			_logProvider.LogException(context.Exception, args);
		}
	}
}