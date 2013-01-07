using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Core
{
	public interface IHistoryStore
	{
		void AddToHistory(UserId userId, string sqlStatement, int evaluationResult, bool completesLesson);
	}
}
