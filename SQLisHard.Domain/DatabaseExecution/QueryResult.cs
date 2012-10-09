using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain.DatabaseExecution
{
    public class QueryResult : Query
    {
        public DataTable Data { get; private set; }

        public QueryResult()
        {
            Data = new DataTable();
        }
    }

    public class DataTable
    {
        public Dictionary<int, DataColumnHeader> _headers;
        public int _maxColId;
        public List<DataRow> _rows;

        public void AddHeader(int columnId, string columnName, string columnType)
        {
            _headers.Add(columnId, new DataColumnHeader(columnId, columnName, columnType));
        }

        public DataRow AddRow()
        {
            return new DataRow(_maxColId);
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

    public class DataRow { }

}
