using SQLisHard.Domain.Exercises;
using SQLisHard.Domain.Exercises.ExerciseStore;
using SQLisHard.Domain.QueryEngine.DatabaseExecution;
using SQLisHard.General.ExperienceLogging.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
		public static IExerciseStore ExerciseStore { get; private set; }

#if DEBUG
        private FileSystemWatcher _fw;
#endif

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

			var store = new FlatFileExerciseStore(new QueryEngine(ConfigurationManager.ConnectionStrings["SampleDatabase"].ConnectionString));
            foreach (var file in Directory.EnumerateFiles(Server.MapPath("Exercises")))
            {
                store.Add(File.ReadAllText(file));
            }
			ExerciseStore = store;

#if DEBUG
            _fw = new FileSystemWatcher(Server.MapPath("Exercises"),"*.txt");
            _fw.EnableRaisingEvents = true;
            _fw.Changed += ExerciseChanged;
#endif
        }

#if DEBUG
        private void ExerciseChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (ExerciseStore is FlatFileExerciseStore)
                {
                    ((FlatFileExerciseStore)ExerciseStore).Add(File.ReadAllText(e.FullPath));
                }
            }
            catch
            { }
        }
#endif

        protected void Session_Start(Object sender, EventArgs e)
		{
			// http://stackoverflow.com/questions/2874078/asp-net-session-sessionid-changes-between-requests
			Session["seriously?"] = "yeah, seriously";
		}

		private void SetDefaultLogProvider()
		{
			// switching to use the multiprovider which performs NullProvider services when nothing else is available
			var provider = new MultiProvider();
			Logger.SetDefaultLogger(provider);

			var environmentName = ConfigurationManager.AppSettings["Environment.Name"] + ".sqlishard.com";
			var version = ConfigurationManager.AppSettings["Application.Version"];

			//// Storm if settings available
			//// only uses storm on text and live servers so I don't have to commit the acces token or provider
			////	will figure out a local solution later
			//var stormBaseUrl = ConfigurationManager.AppSettings["Storm.BaseUrl"];
			//var stormAccessToken = ConfigurationManager.AppSettings["Storm.AccessToken"];
			//var stormProjectId = ConfigurationManager.AppSettings["Storm.ProjectId"];

			//if (!string.IsNullOrWhiteSpace(stormAccessToken) && stormAccessToken != "off")	//special hardcoded value because ms deploy params can't be empty
			//	provider.AddProvider(new StormProvider(stormBaseUrl, stormAccessToken, stormProjectId, environmentName, version, true));

			//// Scalyr if settings are available
			//var scalyrBaseUrl = ConfigurationManager.AppSettings["Scalyr.BaseUrl"];
			//var scalyrWriteToken = ConfigurationManager.AppSettings["Scalyr.WriteToken"];
			//var scalyrSession = Environment.MachineName;

			//if (!string.IsNullOrWhiteSpace(scalyrWriteToken) && scalyrWriteToken != "off")	//special hardcoded value because ms deploy params can't be empty
			//	provider.AddProvider(new ScalyrProvider(scalyrBaseUrl, scalyrWriteToken, environmentName, version, scalyrSession, true));

		}
	}
}