using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLisHard.General.ExperienceLogging.Communications;

namespace SQLisHard.General.ExperienceLogging.Log
{
	public interface IExperienceLogProvider
	{
		void Log(Dictionary<string, object> message, Action<Result>? callback);
	}
}
