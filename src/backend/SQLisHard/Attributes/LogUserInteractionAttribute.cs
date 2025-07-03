using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SQLisHard.General.ExperienceLogging.Log;

namespace SQLisHard.Attributes
{
	// TODO: replace this filter approach with a 3rd party lib or middleware -> queue or by adding data for an observability sln
	public class LogUserInteractionAttribute : ActionFilterAttribute
	{
		private const string INTERACTION_LOGGER_KEY = "ElapsedCaptureObject";
        private readonly IExperienceLogProvider _logProvider;

        public LogUserInteractionAttribute(IExperienceLogProvider logProvider)
        {
			_logProvider = logProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			var controllerName = ((ControllerBase)actionContext.Controller)
			   .ControllerContext.ActionDescriptor.ControllerName;
			var actionName = ((ControllerBase)actionContext.Controller)
			   .ControllerContext.ActionDescriptor.ActionName;

			var data = new Dictionary<string, object>() {
				{"EndpointType", "??"},
				{"Controller", controllerName},
				{"Action", actionName},
				// {"Environment", ConfigurationManager.AppSettings["Environment.Name"]}
			};
			actionContext.HttpContext.Items[INTERACTION_LOGGER_KEY] = new LogDetailsWithElapsedTime(data);
		}

		public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
		{
			var elapsedCaptureObject = actionExecutedContext.HttpContext.Items[INTERACTION_LOGGER_KEY] as LogDetailsWithElapsedTime;
			if (elapsedCaptureObject != null) // should always be the case, but better safe then sorry, wouldn't be the first time I was wrong
			{
				string result = "success";
				if (actionExecutedContext.Exception != null)
					result = "exception " + actionExecutedContext.Exception.GetType().Name;
				elapsedCaptureObject.Add("Result", result);

				if (actionExecutedContext.HttpContext != null)
				{
					//elapsedCaptureObject.Add("SessionId", actionExecutedContext.HttpContext.Session.Id);

					if (actionExecutedContext.HttpContext.User.Identity?.IsAuthenticated == true)
					{
						elapsedCaptureObject.Add("UserId", actionExecutedContext.HttpContext.User.FindFirst("id")?.Value ?? "unknown");
					}

					if (actionExecutedContext.HttpContext.Items.ContainsKey("AdditionalInteractionValues") && actionExecutedContext.HttpContext.Items["AdditionalInteractionValues"] is Dictionary<string, string>)
					{
						var additionalValues = (Dictionary<string, string>)actionExecutedContext.HttpContext.Items["AdditionalInteractionValues"]!;
						foreach (var key in additionalValues.Keys)
						{
							elapsedCaptureObject.Add(key, additionalValues[key]);
						}
					}
				}

				var details = elapsedCaptureObject.Complete();
				_logProvider.Log(details, null);
			}
		}
	}
}