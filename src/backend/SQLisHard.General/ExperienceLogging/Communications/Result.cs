using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.Json;

namespace SQLisHard.General.ExperienceLogging.Communications
{
	public class Result
	{
		public bool Success { get; set; }
		public string? RawContent { get; set; }

		public T? GetContent<T>()
		{
			if (string.IsNullOrEmpty(RawContent))
			{
				return default(T);
			}
			try
			{
				return JsonSerializer.Deserialize<T>(RawContent);
			}
			catch (JsonException ex)
			{
				System.Diagnostics.Trace.WriteLine($"JSON Deserialization Error: {ex.Message}");
				return default(T);
			}
		}
	}
}
