using System;
using NUnit.Framework;
using Socrata.Data;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture()]
	public class ColumnUnitTest
	{
		[Test()]
		public void TestEndpointUrl()
		{
			View dummyView = new View();
			dummyView.id = "aoeu-aoeu";
			Column collycol = new Column();
			collycol.id = 42;
			collycol.Parent = dummyView;

			var columnUrl = collycol.ResourceUrl();
			
			Assert.AreEqual(dummyView.ResourceUrl() + "/columns/" + collycol.ResourceId(), columnUrl);
		}
		
		[Test]
		public void TestDataTypeDeserialization()
		{
			Column c = new Column();
			c.dataTypeName = "text";
			Assert.AreEqual(c.Type, Column.DataType.Text);
		}
	}
}
