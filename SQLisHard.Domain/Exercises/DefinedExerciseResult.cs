using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.Exercises
{
	public class DefinedExerciseResult
	{
		private QueryResult _rawQueryResult;

		protected DefinedExerciseResult() { }

		public DefinedExerciseResult(QueryResult queryResult)
		{
			_rawQueryResult = queryResult;
		}

		public override bool Equals(object obj)
		{
			if (obj is QueryResult)
			{
				var userQuery = (QueryResult)obj;
				if(userQuery.TotalRowCount != _rawQueryResult.TotalRowCount)
					return false;

				if (userQuery.Data.Rows.Count < _rawQueryResult.Data.Rows.Count)
					return false;

				if (userQuery.Data.Rows.Count > _rawQueryResult.Data.Rows.Count && userQuery.Data.Rows.Count != _rawQueryResult.TotalRowCount)
					return false;

				if (userQuery.Data.Headers.Length != _rawQueryResult.Data.Headers.Length)
					return false;

				var userHeaders = String.Join("**", userQuery.Data.Headers.Select(h => h.ColumnId + ":" + h.ColumnName + ":" + h.ColumnType));
				var rawHeaders = String.Join("**", _rawQueryResult.Data.Headers.Select(h => h.ColumnId + ":" + h.ColumnName + ":" + h.ColumnType));
				if (!userHeaders.Equals(rawHeaders, StringComparison.CurrentCultureIgnoreCase))
					return false;

				var rowsToCompare = Math.Min(userQuery.Data.Rows.Count, _rawQueryResult.Data.Rows.Count);
				for (int r = 0; r < rowsToCompare; r++)
				{
					for (int c = 0; c < userQuery.Data.Headers.Length; c++)
					{
						if (!userQuery.Data.Rows[r].Values[c].Equals(_rawQueryResult.Data.Rows[r].Values[c]))
							return false;
					}
				}

				return true;
			}

			return false;
		}
	}
}
