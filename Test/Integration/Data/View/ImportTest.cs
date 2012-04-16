using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class ImportTest : TestHelper
	{
		private static int FixtureRowCount = 28;
		private string[] expectedColumnNames = {"A", "Bee", "See"};
		private string[] expectedColumnTypes = {"number", "text", "number"};
		private object[] expectedFirstRow    = {"1", "Hello, World", "3.5"};
		
		private void VerifyColumnsInView(View view, string[] names, string[] types)
		{
			Expect (view, Is.Not.Null, "should get a view back from imports service");
			Expect (view.Columns().Count, Is.AtLeast(names.Length));
			
			int index = 0;
			foreach(var col in view.Columns())
			{
				if (col.name.Equals(names[index]))
				{
					Expect(col.Type.ToString().ToLower(), Is.EqualTo(types[index]),
						String.Format ("Column '{0}' should be of type '{1}'", col.name, types[index]));
					index++;
				}
			}
			Expect (index, Is.EqualTo(names.Length), "Should have found all the expected column names");
		}
		
		[Test]
		public void TestImportSimpleCsvSpecifyingColumnsBeforehand()
		{
			View v = TestHelper.GetEmptyView("CSV Import Integration", false);
			string location = TestHelper.GetFixtureNamed("simple_import.csv");

			createColumn("A", v, Column.DataType.Number);
			createColumn("Bee", v);
			createColumn("See", v, Column.DataType.Number);
			View imported = v.ImportFile(location, 1);
			VerifyNoImportWarnings(v);

			Expect (imported.name, Is.EqualTo (v.name), "should use the name provided for the view");

			VerifyColumnsInView(imported, expectedColumnNames, expectedColumnTypes);

			imported.Delete();
		}
		
		[Test]
		public void TestImportCsvUseScannedColumnSuggestions()
		{
			View v = TestHelper.GetEmptyView("CSV Import Integration, Scanned Columns", false);
			string location = TestHelper.GetFixtureNamed("simple_import.csv");

			View imported = v.ImportFile(location, 1);
			VerifyNoImportWarnings(v);
			
			VerifyColumnsInView(imported, expectedColumnNames, expectedColumnTypes);

			imported.Delete();		
		}
		
		[Test]
		public void TestImportSkipsHeaderWhenAskedNicely()
		{
			View v = TestHelper.GetEmptyView("CSV Import Integration, Scanned Columns", false);
			string location = TestHelper.GetFixtureNamed("simple_import.csv");

			View imported = v.ImportFile(location, 1);
			VerifyNoImportWarnings(v);
		
			var rows = imported.GetRows(0, 100);
			Expect (rows.Count, Is.EqualTo(FixtureRowCount - 1));
			
			// find where the column starts
			Expect (imported.Columns().Count, Is.GreaterThanOrEqualTo(expectedColumnNames.Length));
			
			var expectedRows = new System.Collections.Generic.List<object[]>();
			expectedRows.Add(expectedFirstRow);
			TestHelper.VerifyRows(imported, expectedColumnNames, expectedRows);
			imported.Delete();
		}
		
		[Test]
		public void TestMoreComplicatedImport()
		{
			View v = GetEmptyView("CSV Complex Import", false);
			string file = GetFixtureNamed("complex_import.csv");
			
			v = v.ImportFile(file, 1);
			VerifyNoImportWarnings(v);
			
			var rows = v.GetRows(0, 10);
			Expect (rows.Count, Is.EqualTo (10));
			
			var columns = v.Columns();
			Expect (columns.Count, Is.EqualTo(4));
			
			string[] types = {"calendar_date", "percent", "location", "money"};
			string[] names = {"Date", "Confidence", "Address", "Money"};
			VerifyColumnsInView(v, names, types);
			
			v.Delete();
		}
	}
}
