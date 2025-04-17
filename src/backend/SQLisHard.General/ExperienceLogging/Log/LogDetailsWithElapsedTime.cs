using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLisHard.General.ExperienceLogging.Communications;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace SQLisHard.General.ExperienceLogging.Log
{

	public class LogDetailsWithElapsedTime
	{

		private Dictionary<string, object> _message;
		private Stopwatch _timer;

		public LogDetailsWithElapsedTime(Dictionary<string, object> initialMessage)
		{
			_message = initialMessage;
			_message.Add("StartTime", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
			_timer = new Stopwatch();
			_timer.Start();
		}

		public Dictionary<string, object> Complete()
		{
			_message.Add("ElapsedTime", _timer.ElapsedMilliseconds);
			return _message;
		}

		public void Add(string newKey, string newMessage)
		{
			_message.Add(newKey, newMessage);
		}
	}
}
