using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLisHard.General.ExperienceLogging.Communications;

namespace SQLisHard.General.ExperienceLogging.Log
{
	public interface ILogProvider
	{
		void Log(Dictionary<string, string> message, Action<Result> callback);
	}
}
