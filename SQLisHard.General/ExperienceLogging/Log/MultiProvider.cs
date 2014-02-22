using SQLisHard.General.ExperienceLogging.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.General.ExperienceLogging.Log
{
	public class MultiProvider : ILogProvider
	{
		private List<ILogProvider> _providers;

		public MultiProvider()
		{
			_providers = new List<ILogProvider>();
		}

		public void AddProvider(ILogProvider additionalProvider) 
		{
			_providers.Add(additionalProvider);
		}

		public void Log(Dictionary<string, object> message, Action<Communications.Result> callback)
		{
			if (_providers.Count > 0)
			{
				var results = new List<Result>();
				var resultsLock = new object();
				int expectedCount = _providers.Count;
				foreach (var provider in _providers)
				{
					provider.Log(message, (result) =>
					{
						lock (resultsLock)
						{
							results.Add(result);
							if (results.Count == expectedCount && callback != null)
							{
								var collectiveResult = new Result();
								collectiveResult.Success = results.All(r => r.Success == true);
								collectiveResult.RawContent = string.Join("\n\n", results.Select(r => r.RawContent));
								callback(collectiveResult);
							}
						}
					});
				}
			}
			else
			{
				if(callback != null)
					callback(new Result() { Success = true });
			}
		}
	}
}
