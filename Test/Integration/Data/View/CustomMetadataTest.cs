using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class CustomMetadataTest : AssertionHelper
	{
		private View view;
		
		[SetUp]
		public void Init()
		{
			view = TestHelper.GetEmptyView("Metadata Integration");
		}
		
		
		[Test]
		public void TestPersistCustomFields()
		{
			string fieldsetName = "Foo_+ Bar";
			string fieldName = "Baz";
			string fieldValue = "noop";
			
			var dumbValues = new Dictionary<string, string>();
			dumbValues.Add(fieldName, fieldValue);
			view.CustomFields[fieldsetName] = dumbValues;
			view.Update();

			View fromId = View.FromId(view.id);
			
			Expect(fromId.CustomFields, Is.Not.Null, "should have custom fields defined");
			
			Expect(fromId.CustomFields, Is.AssignableFrom(typeof(Dictionary<string, object>)));
			Expect(fromId.CustomFields[fieldsetName], Is.Not.Null);
			
			Expect(fromId.CustomFields[fieldsetName], Is.AssignableFrom(typeof(Dictionary<string, object>)));

			
			Expect(fromId.MetadataFieldset(fieldsetName), Is.Not.Null, "should have a metadata fieldset");
			
			Expect((string)fromId.MetadataFieldset(fieldsetName)[fieldName], Is.EqualTo(fieldValue), "value should be what we set it to");
		}
		
		[TearDown]
		public void Dispose()
		{
			view.Delete ();
		}
	}
}
