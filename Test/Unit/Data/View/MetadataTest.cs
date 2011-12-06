using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class MetadataUnitTest
	{
		private View view;
		
		[SetUp]
		public void Init()
		{
			view = new View();
			view.name = "Metadata Test " + System.Guid.NewGuid().ToString();
		}
		
		[Test]
		public void TestGetNonExistentField()
		{
			try {
				view.MetadataFieldset("shouldn't exist");
				Assert.Fail("shouldn't work");
			}
			catch (ArgumentException) {}
		}
		
		[Test]
		public void TestSetAndAccessCustomFields()
		{
			var myFieldset = new Dictionary<string, object>();
			myFieldset["Custom Field"] = "Custom Value";
			view.CustomFields["foo bar"] = myFieldset;
			
			Assert.That(view.CustomFields, Is.Not.Null);
			Assert.That(view.MetadataFieldset("foo bar"), Is.Not.Null);
			Assert.That((string)view.MetadataFieldset("foo bar")["Custom Field"], Is.EqualTo("Custom Value"));
		}
	}
}