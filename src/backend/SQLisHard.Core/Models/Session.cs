using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Core.Models
{
	public class Session
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string UserAgent { get; set; }
		public string RemoteAddress { get; set; }

		public Session(UserId userId, string userAgent, string remoteAddress)
		{
			UserId = userId.Value;
			UserAgent = userAgent;
			RemoteAddress = remoteAddress;
		}
	}
}
