using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Dynamic;
using System.Reflection;

namespace SQLisHard.IntegrationTests.Configs
{
	class Settings
	{
		public string BaseUrl { get; private set; }
		public string FirstExerciseQuery { get; private set; }
		public string FirefoxBinaryPath { get; set; }
		public string PatternExerciseId { get; private set; }

		private static Settings _settings;
		public static Settings CurrentSettings
		{
			get
			{
				if (_settings == null)
				{
					_settings = LoadSettings("Configs/TestRun.config");
				}
				return _settings;
			}
		}

		private Settings(string url)
		{
			BaseUrl = url;
		}

		private static Settings LoadSettings(String file)
		{
			var settingsFile = XElement.Load(file);
			string url = settingsFile.Element("BaseURL").Value;
			var settings = new Settings(url);
			settings.FirstExerciseQuery = settingsFile.Element("FirstExerciseQuery").Value;
			settings.PatternExerciseId = settingsFile.Element("PatternExerciseId").Value;

			return settings;
		}


	}
}
