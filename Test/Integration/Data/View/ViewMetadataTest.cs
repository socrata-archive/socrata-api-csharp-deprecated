using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class ViewMetadataTest : TestHelper
	{
		[Test]
		public void TestPublicPrivateToggle()
		{
			var saved = GetEmptyView("Setting public");
			
			Expect (saved.IsPublic(), Is.False, "views should start private if not explicitly made public");
			
			saved.SetPublic(true);
			
			saved = View.FromId(saved.ResourceId());
			Expect (saved.IsPublic(), Is.True, "view should be public after marking it as such");
			
			saved.SetPublic(false);
			saved = View.FromId(saved.ResourceId());
			Expect (saved.IsPublic(), Is.False, "view should be private after marking it as such");
	
			saved.Delete();
		}
		
		[Test]
		public void TestSetDescriptionAndReadItBack()
		{
			var view = GetEmptyView("Setting description", false);
			view.description = "Some description";
			var saved = view.Create();
			
			Expect (saved.description, Is.EqualTo(view.description));
			
			var retrieved = View.FromId(saved.ResourceId());
			
			Expect (retrieved.description, Is.EqualTo(view.description));

			saved.Delete();
		}
		
		[Test]
		public void TestUserTagsAkaTopics()
		{
			var view = GetEmptyView("User tags", false);
			view.Topics().Add("test_one_two");
			view = view.Create();
			
			Expect (view.Topics().Contains ("test_one_two"), Is.True, "should contain the tag we added");
			
			view.Delete();
		}
	}
}

