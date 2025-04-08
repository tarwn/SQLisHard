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

		public QueryEngine(string sampleConnectionString)
		{
			_connectionString = sampleConnectionString;
		}

		public QueryResult ExecuteQuery(Query query, bool includeStatistics)
		{
			//TODO connection errors not captured as part of reslut - need ability to capture result that is not user error
			using (var conn = new SqlConnection(_connectionString))
			{
				// move this outside one day + capture error bool on statements, this is proof of concept
				List<string> infoMessages = new List<string>();
				conn.InfoMessage += (sender, e) =>
				{
					infoMessages.Add(e.Message);
				};
				//conn.FireInfoMessageEventOnUserErrors = true;
				var command = new SqlCommand(query.Content, conn);

				if (includeStatistics)
					command.CommandText = "SET STATISTICS IO ON; " + command.CommandText;

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

						// spin through remainder of result sets so we can consume messages
						while (reader.NextResult()) { }

						reader.Close();

						if (query.LimitResults && result.TotalRowCount > result.Data.Rows.Count)
							result.IsSubsetOfRows = true;

						result.InfoMessages.AddRange(infoMessages);
						return result;
					}
				}
				catch (SqlException sqlException)
				{
					var errors = sqlException.Errors.Cast<SqlError>();
					if (errors.Any(e => Convert.ToInt32(e.State) > 16		// anything not user correctable
										&& Convert.ToInt32(e.State) != 20	// no permissions to drop table
										&& Convert.ToInt32(e.State) != 62))	// bad stored proc name error
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
