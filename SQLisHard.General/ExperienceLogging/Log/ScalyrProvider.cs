﻿using ServiceStack.Text;
using SQLisHard.General.ExperienceLogging.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.General.ExperienceLogging.Log
{
	public class ScalyrProvider : ILogProvider
	{
		private string _baseUrl;
		private string _accessToken;
		private bool _sendAsync;
		private string _host;
		private string _version;
		private string _session;

		public ScalyrProvider(string baseUrl, string accessToken, string host, string version, string session, bool sendAsync = true)
		{
			_baseUrl = baseUrl;
			_accessToken = accessToken;
			_sendAsync = sendAsync;
			_host = host;
			_version = version;
			_session = session;
		}

		public string FullUrl
		{
			get
			{
				return _baseUrl;
			}
		}

		public void Log(Dictionary<string, object> message, Action<Communications.Result> callback)
		{

			message["version"] = _version;

			var evt = new ScalyrEvent(DateTime.UtcNow);
			foreach (var pair in message.Select(kvp => kvp))
				evt.attrs[pair.Key] = pair.Value.ToString();

			var messageObject = new ScalyrMessage() {
				token = _accessToken,
				session = _session
			};
			messageObject.sessionInfo.serverId = _host;
			messageObject.events.Add(evt);

			string messageString = JsonSerializer.SerializeToString(messageObject);

			var request = new HttpJsonPost(messageString, null, true);
			if (_sendAsync)
				request.SendAsync(FullUrl, "POST", callback);
			else
				request.Send(FullUrl, "POST", callback);
		}
	}

	public class ScalyrMessage
	{
		public string token { get; set; }
		public string session { get; set; }
		public ScalyrSessionInfo sessionInfo { get; set; }
		public List<ScalyrEvent> events { get; private set; }

		public ScalyrMessage()
		{
			sessionInfo = new ScalyrSessionInfo();
			events = new List<ScalyrEvent>();
		}
	}

	public class ScalyrSessionInfo
	{
		public string serverType { get; set; }
		public string serverId { get; set; }

		public ScalyrSessionInfo()
		{
			serverType = "NoTypeSpecified";
			serverId = "NotSpecified";
		}
	}

	public class ScalyrEvent
	{
		public string ts { get; private set; }
		public Dictionary<string, string> attrs { get; set; }

		public ScalyrEvent(DateTime timestamp)
		{
			ts = (timestamp.Subtract(DateTime.Parse("1/1/1970")).TotalMilliseconds * 1000000).ToString("F0");
			attrs = new Dictionary<string, string>();
		}
	}
}
