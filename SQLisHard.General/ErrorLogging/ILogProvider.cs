using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace SQLisHard.General.ErrorLogging
{
	public interface ILogProvider
	{
		void LogException(Exception exception, LogArguments logArguments);
	}
}
