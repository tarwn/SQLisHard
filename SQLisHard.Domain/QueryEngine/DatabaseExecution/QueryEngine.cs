using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.QueryEngine.DatabaseExecution
{
    public class QueryEngine : IQueryEngine
    {
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
                        QueryResult result = null;
                        while (reader.Read())
                        {
                            if (result == null)
                            {
                                result = new QueryResult(query);
                                for (int col = 0; col < reader.FieldCount; col++)
                                {
                                    result.Data.AddHeader(col, reader.GetName(col), reader.GetDataTypeName(col));
                                }
                            }

                            var row = result.Data.AddRow();
                            reader.GetValues(row.Values);
                        }

                        reader.Close();
                        return result;
                    }
                }
                catch (SqlException sqlException)
                {
                    return new QueryResult(query) 
                    {
                        ExecutionStatus = QueryExecutionStatus.Error,
                        ErrorMessage = sqlException.Message,
                        ErrorNumber = sqlException.Number
                    };
                }
            }
        }
    }
}
