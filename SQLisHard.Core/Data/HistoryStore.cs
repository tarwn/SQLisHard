using PetaPoco;
using SQLisHard.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SQLisHard.Core.Data
{
	public class HistoryStore : IHistoryStore
	{
		private string _connectionString;

		public HistoryStore(string connectionString)
		{
			_connectionString = connectionString;
		}

		public void AddToHistory(UserId userId, string sqlStatement, int evaluationResult, bool completesExercise, string exerciseId)
		{
			using (var db = new Database(_connectionString, "System.Data.SqlClient"))
			{
				db.Insert("History", "Id", true, new {
					UserId = userId.Value,
					SqlStatement = sqlStatement,
					Result = evaluationResult,
					CompletesExercise = completesExercise,
                    ExerciseId = exerciseId
				});
			}
		}
	}
}
