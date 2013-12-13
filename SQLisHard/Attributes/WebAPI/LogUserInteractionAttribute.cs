using SQLisHard.Core;
using SQLisHard.General.ExperienceLogging.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Security;

namespace SQLisHard.Attributes.WebAPI
{

	public class LogUserInteractionAttribute : ActionFilterAttribute
	{
		private const string INTERACTION_LOGGER_KEY = "ElapsedCaptureObject";

		public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			var data = new Dictionary<string, string>() { 
				{"EndpointType", "WebAPI"},
				{"Controller", actionContext.ControllerContext.ControllerDescriptor.ControllerName},
				{"Action", actionContext.ActionDescriptor.ActionName},
				{"Environment", ConfigurationManager.AppSettings["Environment.Name"]}
			};
			actionContext.Request.Properties[INTERACTION_LOGGER_KEY] = Logger.CaptureElapsedTime(data, null);
		}

		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			var elapsedCaptureObject = actionExecutedContext.Request.Properties[INTERACTION_LOGGER_KEY] as LoggerWithElapsedTime;
			if (elapsedCaptureObject != null) // should always be the case, but better safe then sorry, wouldn't be the first time I was wrong
			{
				string result = "success";
				if (actionExecutedContext.Exception != null)
					result = "exception " + actionExecutedContext.Exception.GetType().Name;
				elapsedCaptureObject.Add("Result", result);

				if (HttpContext.Current != null)
				{
					var allTheDamnCookiesBecauseProvidingANameDoesntWork = actionExecutedContext.Request.Headers.GetCookies();
					var cookie = allTheDamnCookiesBecauseProvidingANameDoesntWork.SelectMany(cc => cc.Cookies.Where(c => c.Name == "ASP.NET_SessionId")).FirstOrDefault();
					if (cookie != null)
					{
						elapsedCaptureObject.Add("SessionId", cookie.Value);
					}
		
					if (HttpContext.Current.User.Identity.IsAuthenticated)
					{
						var user = HttpContext.Current.User as UserPrincipal;
						if (user != null)
						{
							elapsedCaptureObject.Add("UserId", user.UserIdentity.Id.ToString());
						}
					}
				}

				if (actionExecutedContext.Request.Properties.ContainsKey("AdditionalInteractionValues") && actionExecutedContext.Request.Properties["AdditionalInteractionValues"] is Dictionary<string, string>)
				{ 
					var additionalValues = (Dictionary<string,string>) actionExecutedContext.Request.Properties["AdditionalInteractionValues"];
					foreach (var key in additionalValues.Keys)
						elapsedCaptureObject.Add(key, additionalValues[key]);
				}

				elapsedCaptureObject.Dispose();
			}
		}
	}
}