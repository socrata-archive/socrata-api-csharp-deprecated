using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class AppendReplaceTest : TestHelper
	{
		static string[] headers = {"One","Two","Three"};
		static string[] firstRow = {"Hello, World!", "42", "5.9"};
		static string[] otherRows = {"Snuffle", "uppa", "data"};

		private View createShell()
		{
			var view = GetEmptyView("Appendectomy and re-appendectomy", false);
			var fixture = GetFixtureNamed("shell.csv", "Append");
			return view.ImportFile(fixture, 1);
		}
		
		[Test]
		public void TestAppendSkipHeaders ()
		{
			var view = createShell();
			TestHelper.VerifyNoImportWarnings(view);
			var rows = view.GetRows(0, 100);
			Expect (rows.Count, Is.EqualTo(1));
			
			view.Append(GetFixtureNamed("shell.csv", "Append"), 1);
			rows = view.GetRows (0, 100);
			Expect (rows.Count, Is.EqualTo(2), "should only append one row, skipping header");
			
			var rowData = new List<object[]>();
			rowData.Add(firstRow);
			rowData.Add(firstRow);
			VerifyRows(view, headers, rowData);
			
			view.Delete ();
		}
		
		[Test]
		public void TestAppendNoSkip()
		{
			var view = createShell();
			TestHelper.VerifyNoImportWarnings(view);
			var rows = view.GetRows(0, 100);
			Expect (rows.Count, Is.EqualTo(1));
			
			view.Append(GetFixtureNamed("two_rows.csv", "Append"));
			rows = view.GetRows (0, 100);
			Expect (rows.Count, Is.EqualTo(3), "should append both rows");

			var rowData = new List<object[]>();
			rowData.Add(firstRow);
			rowData.Add(otherRows);
			rowData.Add(otherRows);
			VerifyRows(view, headers, rowData);	
			
			view.Delete ();
		}
		
		[Test]
		public void TestReplace()
		{
			var view = createShell();
			TestHelper.VerifyNoImportWarnings(view);
			var rows = view.GetRows(0, 100);
			Expect (rows.Count, Is.EqualTo(1));
			
			view.Replace(GetFixtureNamed("two_rows.csv", "Append"));
			rows = view.GetRows (0, 100);
			Expect (rows.Count, Is.EqualTo(2), "should replace and have two rows");

			var rowData = new List<object[]>();
			rowData.Add(otherRows);
			rowData.Add(otherRows);
			VerifyRows(view, headers, rowData);	
			
			view.Delete ();
		}
	}
}
