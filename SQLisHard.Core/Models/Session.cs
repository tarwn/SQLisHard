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
		public string HostAddress { get; set; }

		public Session(UserId userId, string userAgent, string hostAddress)
		{
			// TODO: Complete member initialization
			UserId = userId.Value;
			UserAgent = userAgent;
			HostAddress = hostAddress;
		}
	}
}
