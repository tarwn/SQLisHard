using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SQLisHard.General.ExperienceLogging.Communications;

namespace SQLisHard.General.ExperienceLogging.Log
{
	public class StormProvider : ILogProvider
	{

		private string _baseUrl;
		private string _accessToken;
		private string _projectId;
		private bool _sendAsync;
		private string _host;
		private string _version;

		public StormProvider(string baseUrl, string accessToken, string projectId, string host, string version, bool sendAsync = true)
		{
			_baseUrl = baseUrl;
			_accessToken = accessToken;
			_projectId = projectId;
			_sendAsync = sendAsync;
			_host = host;
			_version = version;
		}

		public string FullUrl
		{
			get
			{
				return string.Format("{0}?index={1}&sourcetype=json_predefined_timestamp&host={2}", _baseUrl, _projectId, Uri.EscapeUriString(_host));
			}
		}

		public void Log(Dictionary<string, object> message, Action<Communications.Result> callback)
		{
			message["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
			message["version"] = _version;
			var credentials = new NetworkCredential("x", _accessToken, "");
			var request = new HttpJsonPost(message, credentials, true);
			if (_sendAsync)
				request.SendAsync(FullUrl, "POST", callback);
			else
				request.Send(FullUrl, "POST", callback);
		}
	}
}
