using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class ColumnTest : TestHelper
	{
		
		[Test]
		public void TestColumnAddWorks()
		{
			View initial = GetEmptyView("Column Integration");
			Column c = new Column();
			c.name = "Some column";
			c.Type = Column.DataType.Number;
			initial.AddColumn(c);
			
			Expect(initial.Columns().Count, Is.EqualTo(1), "should have just the one column");
			Expect(initial.Columns()[0].name, Is.EqualTo("Some column"));
			
			View secondary = View.FromId (initial.ResourceId());
			Expect (secondary, Is.Not.Null);
			Expect (secondary.name, StartsWith("Column Integration"), "should have the same name");
			Expect (secondary.Columns().Count, Is.EqualTo(1));
			
			initial.Delete();
		}
		
		[Test]
		public void TestRoundtripWithoutColumnsDoesntDeleteThem()
		{
			View initial = GetEmptyView("Column Integration");
			createColumn("Some column", initial, Column.DataType.Number);

			Expect(initial.Columns().Count, Is.EqualTo(1), "should have just the one column");
			Expect(initial.Columns()[0].name, Is.EqualTo("Some column"));
			
			
			View secondary = View.FromId (initial.ResourceId());
			Expect (secondary.Columns().Count, Is.EqualTo(1));
			secondary.columns = null;

			secondary.Update();
			
			secondary = View.FromId (initial.ResourceId());
			Expect (secondary.Columns(), Is.Not.Null, "nulling out the columns and roundtripping shouldn't delete them");
			Expect (secondary.Columns().Count, Is.EqualTo(1), "nulling out the columns and roundtripping shouldn't delete them");
			
			initial.Delete();
		}
		
		[Test]
		public void TestDeleteColumn()
		{
			View initial = GetEmptyView("Column Deletion");
			createColumn("Some column", initial, Column.DataType.Number);
			Expect(initial.Columns().Count, Is.EqualTo(1), "should have just the one column");
			Expect(initial.Columns()[0].name, Is.EqualTo("Some column"));

			initial.Columns()[0].Delete();
			
			initial = View.FromId(initial.ResourceId());
			Expect (initial.Columns().Count, Is.EqualTo(0), "column should have been deleted");
			
			initial.Delete ();
		}
	}
}

