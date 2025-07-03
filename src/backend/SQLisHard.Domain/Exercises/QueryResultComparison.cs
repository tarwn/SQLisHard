using System.Collections.Generic;

namespace SQLisHard.Domain.Exercises
{
    public class QueryResultComparison
    {
        public bool TotalRowCountMismatch { get;  set; }
        public bool ReturnedRowCountMismatch { get;  set; }
        public bool SomeOtherRowCountMismatch { get;  set; }
        public bool ColumnCountMismatch { get;  set; }
        public bool ExactColumnMismatch { get; set; }
        public bool HasUnrecognizedColumn { get;  set; }
        public List<string> UnrecognizedColumns { get;  set; }

        public bool DataMismatch { get; set; }

        public bool IsMatch
        {
            get
            {
                return !(TotalRowCountMismatch ||
                         ReturnedRowCountMismatch ||
                         SomeOtherRowCountMismatch ||
                         ColumnCountMismatch ||
                         ExactColumnMismatch ||
                         HasUnrecognizedColumn ||
                         DataMismatch);
            }
        }

    }
}