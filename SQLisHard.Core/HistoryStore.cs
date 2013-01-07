using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Core
{
	public class HistoryStore : IHistoryStore
	{
		private string _connectionString;

		public HistoryStore(string connectionString)
		{
			_connectionString = connectionString;
		}

		public void AddToHistory(UserId userId, string sqlStatement, int evaluationResult, bool completesLesson)
		{
			//TODO: implement history store
		}
	}
}
