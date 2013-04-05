using SQLisHard.General.ExperienceLogging.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SQLisHard
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			GlobalConfiguration.Configuration.Filters.Add(new Attributes.WebAPI.CaptureUnhandledExceptionAttribute());
			GlobalConfiguration.Configuration.Filters.Add(new Attributes.WebAPI.LogUserInteractionAttribute());
			GlobalFilters.Filters.Add(new Attributes.MVC.CaptureUnhandledExceptionAttribute());
			GlobalFilters.Filters.Add(new Attributes.MVC.LogUserInteractionAttribute());

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			SetDefaultLogProvider();
		}

		private void SetDefaultLogProvider()
		{
			// only uses storm on text and live servers so I don't have to commit the acces token or provider
			//	will figure out a local solution later
			var stormBaseUrl = ConfigurationManager.AppSettings["Storm.BaseUrl"];
			var stormAccessToken = ConfigurationManager.AppSettings["Storm.AccessToken"];
			var stormProjectId = ConfigurationManager.AppSettings["Storm.ProjectId"];
			var environmentName = ConfigurationManager.AppSettings["Environment.Name"] + ".sqlishard.com";
			var version = ConfigurationManager.AppSettings["Application.Version"];

			ILogProvider provider;
			if (string.IsNullOrWhiteSpace(stormAccessToken) || stormAccessToken == "off")	//special hardcoded value becaue ms deploy params can't be empty
				provider = new NullLogProvider();
			else
				provider = new StormProvider(stormBaseUrl, stormAccessToken, stormProjectId, environmentName, version, true);
			Logger.SetDefaultLogger(provider);
		}
	}
}