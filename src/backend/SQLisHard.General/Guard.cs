using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.General
{
    public static class Guard
    {

		public static void AgainstNullArgument(string argumentName, object value)
		{
			if (value == null)
				throw new ArgumentNullException(argumentName);
		}

    }
}
