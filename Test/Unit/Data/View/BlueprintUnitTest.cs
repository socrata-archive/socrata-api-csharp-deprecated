using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class BlueprintUnitTest : AssertionHelper
	{
		private View getDumbView()
		{
			View v = new View();
			v.name = "Hello, world!";
			v.description = "Goodbye, cruel world";
			v.columns = new List<Column>();
			return v;
		}
		[Test]
		public void TestSimpleBlueprint()
		{
			Blueprint bp = getDumbView().blueprint(2);
			Expect (bp.skip, Is.EqualTo(2));
			
			Expect (bp.name, Is.EqualTo ("Hello, world!"));
		}
		
		[Test]
		public void TestBlueprintColumns()
		{
			View v = getDumbView();
			for(var i = 0; i < 10; i++)
			{
				var col = new Column();
				v.name = "Column " + i.ToString();
				col.Type = Column.DataType.Text;
				v.Columns().Add(col);
			}
			
			Blueprint bp = v.blueprint(0);
			Expect (bp.columns.Count, Is.EqualTo (10));
			foreach(var col in bp.columns)
			{
				Expect (col.datatype, Is.EqualTo(Column.DataType.Text.ToString().ToLower()));
			}
		}
	}
}

