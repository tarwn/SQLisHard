using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain.QueryEngine
{
    public class QueryResult : Query
    {
        public QueryExecutionStatus ExecutionStatus { get; set; }

        public DataTable Data { get; private set; }

        public int TotalRowCount { get; set; }

        public bool IsSubsetOfRows { get;  set; }

        public string ErrorMessage { get; set; }

        public int ErrorNumber { get; set; }


        public QueryResult()
        {
            Data = new DataTable();
        }

        public QueryResult(Query originalQuery) : base(originalQuery)
        {
            Data = new DataTable();
        }

        public QueryResult(QueryResult anotherResult)
            : base(anotherResult)
        {
            Content = anotherResult.Content;
            Data = anotherResult.Data;
            TotalRowCount = anotherResult.TotalRowCount;
            IsSubsetOfRows = anotherResult.IsSubsetOfRows;
            ExecutionStatus = anotherResult.ExecutionStatus;
            ErrorMessage = anotherResult.ErrorMessage;
            ErrorNumber = anotherResult.ErrorNumber;
        }

    }

    public class DataTable
    {
        private Dictionary<int, DataColumnHeader> _headers = new Dictionary<int,DataColumnHeader>();

        public DataColumnHeader[] Headers { get { return _headers.Values.OrderBy(h => h.ColumnId).ToArray(); } }
        public List<DataRow> Rows { get; protected set;}

        public DataTable()
        {
            Rows = new List<DataRow>();
        } 

        public void AddHeader(int columnId, string columnName, string columnType)
        {
            _headers.Add(columnId, new DataColumnHeader(columnId, columnName, columnType));
        }

        public DataRow AddRow()
        {
            var row = new DataRow(_headers.Count);
            Rows.Add(row);
            return row;
        }
    }

    public class DataColumnHeader
    {
        public int ColumnId { get; set; }
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }

        public DataColumnHeader(int columnId, string columnName, string columnType)
        {
            ColumnId = columnId;
            ColumnName = columnName;
            ColumnType = columnType;
        }
    }

    public class DataRow
    {
        public object[] Values { get; private set; }

        public DataRow(int columnCount)
        {
            Values = new object[columnCount];
        }

    }

}
