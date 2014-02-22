using SQLisHard.Core;
using SQLisHard.General.ExperienceLogging.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SQLisHard.Attributes.MVC
{
	public class LogUserInteractionAttribute : ActionFilterAttribute
	{
		private const string INTERACTION_LOGGER_KEY = "ElapsedCaptureObject";

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var data = new Dictionary<string, object>() { 
				{"EndpointType", "MVC"},
				{"Controller", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName},
				{"Action", filterContext.ActionDescriptor.ActionName},
				/* todo: add in action parameters when I start using them */
				{"Environment", ConfigurationManager.AppSettings["Environment.Name"]},
				{"SessionId", filterContext.RequestContext.HttpContext.Session.SessionID }
			};
			filterContext.HttpContext.Items[INTERACTION_LOGGER_KEY] = Logger.CaptureElapsedTime(data, null);
		}

		public override void OnResultExecuted(ResultExecutedContext filterContext)
		{
			var elapsedCaptureObject = filterContext.HttpContext.Items[INTERACTION_LOGGER_KEY] as LoggerWithElapsedTime;
			if (elapsedCaptureObject != null) // should always be the case, but better safe then sorry, wouldn't be the first time I was wrong
			{
				string result = "success";
				if (filterContext.Exception != null)
					result = "exception " + filterContext.Exception.GetType().Name;
				elapsedCaptureObject.Add("Result", result);

				if (filterContext.HttpContext.User.Identity.IsAuthenticated)
				{
					var user = filterContext.HttpContext.User as UserPrincipal;
					if (user != null)
					{
						elapsedCaptureObject.Add("UserId", user.UserIdentity.Id.ToString());
					}
				}

				elapsedCaptureObject.Dispose();
			}
		}
	}
}