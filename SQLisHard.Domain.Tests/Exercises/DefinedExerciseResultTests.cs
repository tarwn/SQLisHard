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

		private void FillDataTable(DataTable data, int expColumnCount, int expRowCount, string colPrefix = "", string rowPrefix = "")
		{
			for(int colCount = 0; colCount < expColumnCount; colCount++)
				data.AddHeader(colCount, colPrefix + "col_" + colCount.ToString(), "string");

			for(int rowCount = 0; rowCount < expRowCount; rowCount++){
				var row = data.AddRow();
				for(int colCount = 0; colCount < expColumnCount; colCount++)
					row.Values[colCount] = rowPrefix + colCount.ToString();
			}
		}
	}
}
