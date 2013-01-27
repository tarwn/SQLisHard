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
		public Dictionary<string, string> ServerVariables { get; set; }
		public string Username { get; set; }
		public int UserId { get; set; }

		// tests
		public LogArguments()
		{
			Headers = new Dictionary<string, IEnumerable<string>>();
			ServerVariables = new Dictionary<string, string>();
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
			foreach (var key in request.ServerVariables.AllKeys)
			{
				ServerVariables.Add(key, request.ServerVariables[key]);
			}
		}

		// WebAPI
		public LogArguments(HttpRequestMessage requestMessage)
			: this()
		{
			RequestURI = requestMessage.RequestUri.AbsoluteUri;
			Headers = requestMessage.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, new HeaderKeyComparer());

			if (System.Web.HttpContext.Current != null)	// dirty dirty dirty
			{
				foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
				{
					ServerVariables.Add(key, System.Web.HttpContext.Current.Request.ServerVariables[key]);
				}
			}
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
