using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using ServiceStack.Text;

namespace SQLisHard.General.ExperienceLogging.Communications
{
	public class RequestState
	{
		public HttpWebRequest Request { get; set; }
		public Action<Result> Callback { get; set; }
		public bool ProcessResponse { get; set; }
		public int RequestRetryCount { get; set; }
	}
}
