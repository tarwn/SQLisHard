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
        private bool _exactColumnMatchOnly;

        protected DefinedExerciseResult() { }

        public DefinedExerciseResult(QueryResult queryResult, bool exactColumnMatchOnly = false)
        {
            _rawQueryResult = queryResult;
            _exactColumnMatchOnly = exactColumnMatchOnly;
        }

        public override bool Equals(object obj)
        {
            if (obj is QueryResult)
            {
                var result = CompareTo(_rawQueryResult, (QueryResult)obj, true);
                return result.IsMatch;
            }

            return false;
        }

        public virtual QueryResultComparison CompareTo(QueryResult userResult)
        {
            return CompareTo(_rawQueryResult, userResult, _exactColumnMatchOnly);
        }

        public static QueryResultComparison CompareTo(QueryResult exerciseResult, QueryResult userResult, bool exactColumnMatchOnly = false)
        {
            var comparison = new QueryResultComparison();

            if (userResult.TotalRowCount != exerciseResult.TotalRowCount)
            {
                comparison.TotalRowCountMismatch = true;
            }

            if (exerciseResult.Data.Rows.Count > userResult.Data.Rows.Count && exerciseResult.Data.Rows.Count < exerciseResult.TotalRowCount)
            {
                comparison.ReturnedRowCountMismatch = true;
                comparison.DataMismatch = true;
            }

            if (userResult.Data.Rows.Count > exerciseResult.Data.Rows.Count && userResult.Data.Rows.Count != exerciseResult.TotalRowCount)
            {
                comparison.SomeOtherRowCountMismatch = true;
                comparison.DataMismatch = true;
            }

            if (userResult.Data.Headers.Length != exerciseResult.Data.Headers.Length)
            {
                comparison.ColumnCountMismatch = true;
                comparison.DataMismatch = true;
            }

            // Exact match
            var userHeaders = String.Join("**", userResult.Data.Headers.Select(h => h.ColumnId + ":" + h.ColumnName + ":" + h.ColumnType));
            var rawHeaders = String.Join("**", exerciseResult.Data.Headers.Select(h => h.ColumnId + ":" + h.ColumnName + ":" + h.ColumnType));
            if (userHeaders.Equals(rawHeaders, StringComparison.CurrentCultureIgnoreCase))
            {
                // verify row values
                for (int i = 0; i < Math.Min(exerciseResult.Data.Rows.Count, userResult.Data.Rows.Count); i++)
                {
                    for (int j = 0; j < exerciseResult.Data.Headers.Length; j++)
                    {
                        if (!exerciseResult.Data.Rows[i].Values[j].Equals(userResult.Data.Rows[i].Values[j]))
                        {
                            comparison.DataMismatch = true;
                            break;
                        }
                    }
                }
            }
            // if we only allow exact matches, we'e in error
            else if (exactColumnMatchOnly)
            {
                comparison.ExactColumnMismatch = true;
            }
            // Inexact match - matches so far but wasn't an exact match
            else if (comparison.IsMatch /* so far */)
            {
                var remainingUserHeaders = userResult.Data.Headers.ToList();

                foreach (var exerciseHeader in exerciseResult.Data.Headers)
                {
                    // two pass search for matching user column
                    DataColumnHeader userHeader = null;
                    //  Pass 1: exact match on name, type, id
                    for (int j = 0; j < remainingUserHeaders.Count; j++)
                    {
                        if (remainingUserHeaders[j].ColumnId == exerciseHeader.ColumnId &&
                            remainingUserHeaders[j].ColumnName == exerciseHeader.ColumnName &&
                            remainingUserHeaders[j].ColumnType == exerciseHeader.ColumnType)
                        {
                            userHeader = remainingUserHeaders[j];
                            remainingUserHeaders.RemoveAt(j);
                            break;
                        }
                    }
                    //  pass 2: match on type and first row value
                    if (userHeader == null)
                    {
                        for (int j = 0; j < remainingUserHeaders.Count; j++)
                        {
                            if (remainingUserHeaders[j].ColumnType == exerciseHeader.ColumnType &&
                                userResult.Data.Rows[0].Values[remainingUserHeaders[j].ColumnId].Equals(exerciseResult.Data.Rows[0].Values[exerciseHeader.ColumnId]))
                            {
                                userHeader = remainingUserHeaders[j];
                                remainingUserHeaders.RemoveAt(j);
                                break;
                            }
                        }
                    }

                    // verify values and exit at first mismatch
                    if (userHeader != null)
                    {
                        for (int i = 0; i < Math.Min(exerciseResult.Data.Rows.Count, userResult.Data.Rows.Count); i++)
                        {
                            if (!exerciseResult.Data.Rows[i].Values[exerciseHeader.ColumnId].Equals(userResult.Data.Rows[i].Values[userHeader.ColumnId]))
                            {
                                comparison.DataMismatch = true;
                                break;
                            }
                        }
                    }
                }

                // any columns left? these were unrecognized
                if (remainingUserHeaders.Any())
                {
                    comparison.HasUnrecognizedColumn = true;
                    comparison.UnrecognizedColumns = remainingUserHeaders.Select(h => $"Column {h.ColumnId + 1} '{h.ColumnName}'").ToList();
                }
            }

            return comparison;
        }
    }
}
