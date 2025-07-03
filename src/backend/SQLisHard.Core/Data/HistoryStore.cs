using PetaPoco;
using SQLisHard.Core.Models;


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
			using (var db = new Database(_connectionString, "Microsoft.Data.SqlClient"))
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

        public List<string> GetCompletedExercises(UserId id)
        {
            using (var db = new Database(_connectionString, "Microsoft.Data.SqlClient"))
            {
                return db.Fetch<string>("SELECT ExerciseId FROM History WHERE UserId = @0 AND CompletesExercise = 1;", id.Value).ToList();
            }
        }
    }
}
