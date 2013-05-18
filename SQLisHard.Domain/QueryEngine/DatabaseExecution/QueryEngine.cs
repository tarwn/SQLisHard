using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.QueryEngine.DatabaseExecution
{
	public class QueryEngine : IQueryEngine
	{
		private const int ARTIFICIAL_LIMIT = 100;

		private string _connectionString;

		public QueryEngine(string connectionString)
		{
			_connectionString = connectionString;
		}

		public QueryResult ExecuteQuery(Query query)
		{
			//TODO connection errors not captured as part of reslut - need ability to capture result that is not user error
			using (var conn = new SqlConnection(_connectionString))
			{
				var command = new SqlCommand(query.Content, conn);

				conn.Open();
				try
				{
					using (var reader = command.ExecuteReader())
					{
						QueryResult result = new QueryResult(query);
						for (int col = 0; col < reader.FieldCount; col++)
						{
							result.Data.AddHeader(col, reader.GetName(col), reader.GetDataTypeName(col));
						}

						while (reader.Read())
						{
							result.TotalRowCount++;

							if (!query.LimitResults || result.Data.Rows.Count < ARTIFICIAL_LIMIT)
							{
								var row = result.Data.AddRow();
								reader.GetValues(row.Values);
							}
						}

						reader.Close();

						if (query.LimitResults && result.TotalRowCount > result.Data.Rows.Count)
							result.IsSubsetOfRows = true;

						return result;
					}
				}
				catch (SqlException sqlException)
				{
					var errors = sqlException.Errors.Cast<SqlError>();
					if (errors.Any(e => Convert.ToInt32(e.State) > 16))	// anything not user correctable
						throw;
					else
						return new QueryResult(query) {
							ExecutionStatus = QueryExecutionStatus.Error,
							ErrorMessage = sqlException.Message,
							ErrorNumber = sqlException.Number
						};
				}
			}
		}
	}
}
