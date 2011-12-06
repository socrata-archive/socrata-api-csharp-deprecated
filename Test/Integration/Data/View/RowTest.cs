using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class RowTest : TestHelper
	{
	
		private View createShell()
		{
			var view = GetEmptyView("Rows", false);
			var fixture = GetFixtureNamed("row_shell.csv", "Rows");
			return view.ImportFile(fixture, 1);
		}
		
		[Test]
		public void TestCanAddRow ()
		{
			var view = createShell();
			var rows = view.GetRows(0, 100);
			var textColumn = GetColumnNamed(view, "Text");
			var numberColumn = GetColumnNamed(view, "Number");
			
			Row newRow = new Row();
			newRow.Data.Add(numberColumn.ResourceId(), "2");
			newRow.Data.Add(textColumn.ResourceId(), "B");
			view.AddRow(newRow);
			
			var addedRows = view.GetRows (0, 100);
			Expect (addedRows.Count, Is.EqualTo(rows.Count + 1), "should be one more row");
			
			var createdRow = addedRows[addedRows.Count-1];
			Expect (createdRow[textColumn.ResourceId()], Is.EqualTo("B"));
			Expect (createdRow[numberColumn.ResourceId()], Is.EqualTo("2"));
			
			view.Delete();
		}

	}
}
