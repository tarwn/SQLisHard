using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SQLisHard.Domain.DatabaseExecution
{
    public class QueryEngine : IQueryEngine
    {
        private string _connectionString;

        public QueryResult ExecuteQuery(Query query)
        {
            var conn = new SqlConnection(_connectionString);
            var command = new SqlCommand(query.Content, conn);


            using (var reader = command.ExecuteReader())
            {
                QueryResult result = null;
                while (reader.Read())
                {
                    if (result == null)
                    {
                        result = new QueryResult();
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
    }
}
