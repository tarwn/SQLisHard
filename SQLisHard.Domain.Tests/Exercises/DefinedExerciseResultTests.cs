using NUnit.Framework;
using SQLisHard.Domain.Exercises;
using SQLisHard.Domain.QueryEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLisHard.Domain.Tests.Exercises
{
	[TestFixture]
	public class DefinedExerciseResultTests
	{
		[Test]
		public void Equals_MatchingQueryResult_ReturnsTrue()
		{
			var queryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(queryResult.Data, 5, 10);
			var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(rawQueryResult.Data, 5, 10);
			var definedResult = new DefinedExerciseResult(rawQueryResult);

			var result = definedResult.Equals(queryResult);

			Assert.IsTrue(result);
		}

		[Test]
		public void Equals_QueryResultWithDifferentTotalRowCount_ReturnsFalse()
		{
			var queryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(queryResult.Data, 5, 10);
			var rawQueryResult = new QueryResult() { TotalRowCount = 246 };
			FillDataTable(rawQueryResult.Data, 5, 10);
			var definedResult = new DefinedExerciseResult(rawQueryResult);

			var result = definedResult.Equals(queryResult);

			Assert.IsFalse(result);
		}

		[Test]
		public void Equals_QueryResultWithDifferentNumberOfColumns_ReturnsFalse()
		{
			var queryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(queryResult.Data, 5, 10);
			var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(rawQueryResult.Data, 4, 10);
			var definedResult = new DefinedExerciseResult(rawQueryResult);

			var result = definedResult.Equals(queryResult);

			Assert.IsFalse(result);
		}

		[Test]
		public void Equals_QueryResultWithSameNumberButDifferentColumns_ReturnsFalse()
		{
			var queryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(queryResult.Data, 5, 10);
			var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(rawQueryResult.Data, 5, 10, "abc");
			var definedResult = new DefinedExerciseResult(rawQueryResult);

			var result = definedResult.Equals(queryResult);

			Assert.IsFalse(result);
		}

		[Test]
		public void Equals_QueryResultWithDifferentRowData_ReturnsFalse()
		{
			var queryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(queryResult.Data, 5, 10);
			var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(rawQueryResult.Data, 5, 10, rowPrefix: "abc");
			var definedResult = new DefinedExerciseResult(rawQueryResult);

			var result = definedResult.Equals(queryResult);

			Assert.IsFalse(result);
		}

		[Test]
		public void Equals_QueryResultWithFewerResultsThanRaw_ReturnsFalse()
		{
			var queryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(queryResult.Data, 5, 9);
			var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(rawQueryResult.Data, 5, 10);
			var definedResult = new DefinedExerciseResult(rawQueryResult);

			var result = definedResult.Equals(queryResult);

			Assert.IsFalse(result);
		}

		[Test]
		public void Equals_QueryResultWithMoreResultsThanRawNotMatchingTotal_ReturnsFalse()
		{
			var queryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(queryResult.Data, 5, 11);
			var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(rawQueryResult.Data, 5, 10);
			var definedResult = new DefinedExerciseResult(rawQueryResult);

			var result = definedResult.Equals(queryResult);

			Assert.IsFalse(result);
		}

		[Test]
		public void Equals_QueryResultWithMatchingResultsMatchingTotalCount_ReturnsTrue()
		{
			var queryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(queryResult.Data, 5, 123);
			var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(rawQueryResult.Data, 5, 10);
			var definedResult = new DefinedExerciseResult(rawQueryResult);

			var result = definedResult.Equals(queryResult);

			Assert.IsTrue(result);
		}

		[Test]
		public void Equals_QueryResultWithMatchingResultsMatchingTotalCountMismatchedHeaderCase_ReturnsTrue()
		{
			var queryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(queryResult.Data, 5, 123);
			queryResult.Data.Headers[0].ColumnName = queryResult.Data.Headers[0].ColumnName.ToUpper();
			var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
			FillDataTable(rawQueryResult.Data, 5, 10);
			rawQueryResult.Data.Headers[0].ColumnName = rawQueryResult.Data.Headers[0].ColumnName.ToLower();
			var definedResult = new DefinedExerciseResult(rawQueryResult);

			var result = definedResult.Equals(queryResult);

			Assert.IsTrue(result);
		}

        [Test]
        public void CompareTo_MatchingQueryResult_ReturnsTrue()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 10);
            var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(rawQueryResult.Data, 5, 10);

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsTrue(result.IsMatch);
            Assert.IsFalse(result.ColumnCountMismatch);
            Assert.IsFalse(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsFalse(result.ReturnedRowCountMismatch);
            Assert.IsFalse(result.SomeOtherRowCountMismatch);
            Assert.IsFalse(result.TotalRowCountMismatch);
        }

        [Test]
        public void CompareTo_QueryResultWithDifferentTotalRowCount_ReturnsFalse()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 10);
            var rawQueryResult = new QueryResult() { TotalRowCount = 246 };
            FillDataTable(rawQueryResult.Data, 5, 10);

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsFalse(result.IsMatch);
            Assert.IsFalse(result.ColumnCountMismatch);
            Assert.IsFalse(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsFalse(result.ReturnedRowCountMismatch);
            Assert.IsFalse(result.SomeOtherRowCountMismatch);
            Assert.IsTrue(result.TotalRowCountMismatch);
        }

        [Test]
        public void CompareTo_QueryResultWithTooFewColumns_ReturnsFalse()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 10);
            var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(rawQueryResult.Data, 4, 10);

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsFalse(result.IsMatch);
            Assert.IsTrue(result.ColumnCountMismatch);
            Assert.IsTrue(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsFalse(result.ReturnedRowCountMismatch);
            Assert.IsFalse(result.SomeOtherRowCountMismatch);
            Assert.IsFalse(result.TotalRowCountMismatch);
        }

        [Test]
        public void CompareTo_QueryResultWithDifferentlyNamedColumns_ReturnsTrue()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 10);
            var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(rawQueryResult.Data, 5, 10, "abc");

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsTrue(result.IsMatch);
            Assert.IsFalse(result.ColumnCountMismatch);
            Assert.IsFalse(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsFalse(result.ReturnedRowCountMismatch);
            Assert.IsFalse(result.SomeOtherRowCountMismatch);
            Assert.IsFalse(result.TotalRowCountMismatch);
        }

        [Test]
        public void CompareTo_QueryResultWithMatchingColumnsAndDifferentRowData_ReturnsFalse()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 10);
            var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(rawQueryResult.Data, 5, 10, rowPrefix: "abc");

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsFalse(result.IsMatch);
            Assert.IsFalse(result.ColumnCountMismatch);
            Assert.IsTrue(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsFalse(result.ReturnedRowCountMismatch);
            Assert.IsFalse(result.SomeOtherRowCountMismatch);
            Assert.IsFalse(result.TotalRowCountMismatch);
        }


        [Test]
        public void CompareTo_QueryResultWithDifferentlyNamedColumnsAndDifferentRowData_ReturnsFalse()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 10);
            var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(rawQueryResult.Data, 5, 10, "abc", "def");

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsFalse(result.IsMatch);
            Assert.IsFalse(result.ColumnCountMismatch);
            Assert.IsTrue(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsFalse(result.ReturnedRowCountMismatch);
            Assert.IsFalse(result.SomeOtherRowCountMismatch);
            Assert.IsFalse(result.TotalRowCountMismatch);
        }

        [Test]
        public void CompareTo_QueryResultWithFewerResultsThanRaw_ReturnsFalse()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 9);
            var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(rawQueryResult.Data, 5, 10);

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsFalse(result.IsMatch);
            Assert.IsFalse(result.ColumnCountMismatch);
            Assert.IsTrue(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsFalse(result.ReturnedRowCountMismatch);
            Assert.IsTrue(result.SomeOtherRowCountMismatch);
            Assert.IsFalse(result.TotalRowCountMismatch);
        }

        [Test]
        public void CompareTo_QueryResultWithMoreResultsThanRawNotMatchingTotal_ReturnsFalse()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 11);
            var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(rawQueryResult.Data, 5, 10);

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsFalse(result.IsMatch);
            Assert.IsFalse(result.ColumnCountMismatch);
            Assert.IsTrue(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsTrue(result.ReturnedRowCountMismatch);
            Assert.IsFalse(result.SomeOtherRowCountMismatch);
            Assert.IsFalse(result.TotalRowCountMismatch);
        }

        [Test]
        public void CompareTo_QueryResultWithMatchingResultsMatchingTotalCount_ReturnsTrue()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 123);
            var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(rawQueryResult.Data, 5, 10);

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsTrue(result.IsMatch);
            Assert.IsFalse(result.ColumnCountMismatch);
            Assert.IsFalse(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsFalse(result.ReturnedRowCountMismatch);
            Assert.IsFalse(result.SomeOtherRowCountMismatch);
            Assert.IsFalse(result.TotalRowCountMismatch);
        }

        [Test]
        public void CompareTo_QueryResultWithMatchingResultsMatchingTotalCountMismatchedHeaderCase_ReturnsTrue()
        {
            var queryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(queryResult.Data, 5, 123);
            queryResult.Data.Headers[0].ColumnName = queryResult.Data.Headers[0].ColumnName.ToUpper();
            var rawQueryResult = new QueryResult() { TotalRowCount = 123 };
            FillDataTable(rawQueryResult.Data, 5, 10);
            rawQueryResult.Data.Headers[0].ColumnName = rawQueryResult.Data.Headers[0].ColumnName.ToLower();

            var result = DefinedExerciseResult.CompareTo(queryResult, rawQueryResult);

            Assert.IsTrue(result.IsMatch);
            Assert.IsFalse(result.ColumnCountMismatch);
            Assert.IsFalse(result.DataMismatch);
            Assert.IsFalse(result.HasUnrecognizedColumn);
            Assert.IsFalse(result.ReturnedRowCountMismatch);
            Assert.IsFalse(result.SomeOtherRowCountMismatch);
            Assert.IsFalse(result.TotalRowCountMismatch);
        }

        private void FillDataTable(DataTable data, int expColumnCount, int expRowCount, string colPrefix = "", string rowPrefix = "")
		{
			for(int colCount = 0; colCount < expColumnCount; colCount++)
				data.AddHeader(colCount, colPrefix + "col_" + colCount.ToString(), "string");

			for(int rowCount = 0; rowCount < expRowCount; rowCount++){
				var row = data.AddRow();
                for (int colCount = 0; colCount < expColumnCount; colCount++)
                {
                    // match data on first row to fool lazy column matching
                    if(rowCount == 0)
                        row.Values[colCount] = colCount.ToString();
                    else
                        row.Values[colCount] = rowPrefix + colCount.ToString();
                }
			}
		}
	}
}
