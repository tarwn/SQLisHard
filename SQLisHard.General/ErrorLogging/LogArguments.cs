using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SQLisHard.General.ErrorLogging
{
	public class LogArguments
	{
		private System.Web.HttpRequestBase httpRequestBase;

		public string RequestURI { get; set; }
		public Dictionary<string, IEnumerable<string>> Headers { get; set; }
		public string Username { get; set; }
		public int UserId { get; set; }

		// tests
		public LogArguments()
		{
			Headers = new Dictionary<string, IEnumerable<string>>();
		}

		// MVC
		public LogArguments(System.Web.HttpRequestBase request)
			: this()
		{
			RequestURI = request.Url.AbsoluteUri;
			foreach (var key in request.Headers.AllKeys)
			{
				Headers.Add(key, new string[] { request.Headers[key] });
			}
		}

		// WebAPI
		public LogArguments(HttpRequestMessage requestMessage)
			: this()
		{
			RequestURI = requestMessage.RequestUri.AbsoluteUri;
			Headers = requestMessage.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, new HeaderKeyComparer());
		}
	}

	class HeaderKeyComparer : IEqualityComparer<string>
	{
		public bool Equals(string x, string y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(string obj)
		{
			return obj.GetHashCode();
		}
	}
}
