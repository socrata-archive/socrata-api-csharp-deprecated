using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class PublishingTest : TestHelper
	{
		[Test]
		public void TestPublish()
		{
			var pub = GetEmptyView("Publishing something", false);
			Expect (pub.Stage(), Is.EqualTo(View.PublicationStage.Unpublished));
		
			pub = pub.Create();
			Expect (pub.Stage(), Is.EqualTo(View.PublicationStage.Unpublished));
			
			pub = pub.Publish();
			Expect (pub.Stage(), Is.EqualTo(View.PublicationStage.Published));
			
			pub.Delete();
		}
		
		[Test]
		public void TestPublishWithLargeFile()
		{
			var v = TestHelper.GetEmptyView("Publish and working copy cycle, large file", false);
			var location = TestHelper.GetFixtureNamed("large_import.csv", "Publish");

			v = v.ImportFile(location, 1);
			
			v = v.Publish ();
			Expect (v.Stage(), Is.EqualTo(View.PublicationStage.Published));
			
			var copy = v.WorkingCopy();
			Expect (copy.Stage(), Is.EqualTo(View.PublicationStage.Unpublished));
			
			v.Delete();
			copy.Delete();
		}
	}
}
