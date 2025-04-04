using SQLisHard.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Core.Data
{
	public interface IHistoryStore
	{
		void AddToHistory(UserId userId, string sqlStatement, int evaluationResult, bool completesExercise, string exerciseId);
		List<string> GetCompletedExercises(UserId id);
	}
}
