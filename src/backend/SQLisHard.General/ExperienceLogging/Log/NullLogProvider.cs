using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SQLisHard.General.ExperienceLogging.Log
{
	public class NullLogProvider : IExperienceLogProvider
	{
		public void Log(Dictionary<string, object> message, Action<Communications.Result>? callback)
		{
			Debug.WriteLine(String.Format("ExperienceLogging: {0}", JsonSerializer.Serialize(message)));

			if (callback != null)
				callback(new Communications.Result { Success = true, RawContent = "{\"success\": true}" });
		}
	}
}
