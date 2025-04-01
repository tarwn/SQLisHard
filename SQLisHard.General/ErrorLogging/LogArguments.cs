using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SQLisHard.General.ErrorLogging
{
	public class LogArguments
	{
		public string RequestURI { get; set; }
		public Dictionary<string, IEnumerable<string>> Headers { get; set; }
		public Dictionary<string, string> ServerVariables { get; set; }
		public string Username { get; set; }
		public int UserId { get; set; }

		// Updated Constructor: Caller must provide all necessary data.
		public LogArguments(string requestUri, Dictionary<string, IEnumerable<string>> headers, Dictionary<string, string> serverVariables, int userId, string username)
		{
			RequestURI = requestUri ?? string.Empty;
			Headers = headers ?? new Dictionary<string, IEnumerable<string>>();
			ServerVariables = serverVariables ?? new Dictionary<string, string>();
			UserId = userId;
			Username = username ?? string.Empty;
		}

		// tests
		public LogArguments()
		{
			Headers = new Dictionary<string, IEnumerable<string>>();
			ServerVariables = new Dictionary<string, string>();
		}
	}
}
