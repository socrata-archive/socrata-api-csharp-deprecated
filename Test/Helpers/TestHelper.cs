using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using Socrata.Server;
using Socrata.Data.View;

namespace SocrataTests
{
	public class TestHelper : AssertionHelper
	{
		public static IConnection GetConnection(string method = "GET")
		{
			IConnection conn = new HttpConnection();
			conn.Method = method;
			return conn;
		}
		
		public static View GetEmptyView(string name = "Test View", bool create = true)
		{
			View view = new View();
			view.name = String.Format("{0} {1}", name, System.Guid.NewGuid().ToString());
			view.Topics().Add("integration");

			return create ? view.Create() : view;
		}
		
		public static string GetFixtureNamed(string name, string type = "Import")
		{
			return String.Format("../../Fixtures/{0}/{1}", type, name);
		}
		
		public static Column GetColumnNamed(View view, string name)
		{
			foreach(var col in view.Columns())
			{
				if (col.name.Equals(name))
				{
					return col;
				}
			}
			Assert.Fail(String.Format("No column named {0} found", name));
			return null;
		}
		
		public static void VerifyRows(View view, string[] columnNames, IList<object[]> data)
		{
			var rows = view.GetRows(0, data.Count + 10);
			Assert.GreaterOrEqual(rows.Count, data.Count, "should get at least as many rows as asked for");
			var columns = new List<Column>(columnNames.Length);
			
			for(int i = 0; i < data.Count; i++)
			{
				var expectedRow = data[i];
				var actualRow = rows[i];
				Assert.IsNotNull(actualRow, "view's row data at index " + i + " should exist");

				for(int j = 0; j < expectedRow.Length; j++)
				{
					if (columns.Count <= j)
					{
						columns.Add(TestHelper.GetColumnNamed(view, columnNames[j]));
					}
					Assert.AreEqual (actualRow[columns[j].id.ToString()], expectedRow[j], "view's row data should match the passed data");
				}
			}
		}
		
		protected static Column createColumn(string name, View view, Column.DataType type = Column.DataType.Text)
		{
			Column column = new Column();
			column.name = name;
			column.Type = type;
			return view.AddColumn(column);
		}
		
		public static void VerifyNoImportWarnings(View view)
		{
			if (view.GetMetadata().ContainsKey("warnings"))
			{
				var warnings = (IList<object>)view.GetMetadata()["warnings"];
				Assert.Fail(String.Format("Got {0} warnings, for example: {1}", warnings.Count, warnings[0]));	
			}
		}
	}
}
