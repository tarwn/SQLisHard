using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using ServiceStack.Text;

namespace SQLisHard.General.ExperienceLogging.Communications
{
	public class Result
	{
		public bool Success { get; set; }
		public string RawContent { get; set; }

		public T GetContent<T>()
		{
			return JsonSerializer.DeserializeFromString<T>(RawContent);
		}
	}
}
