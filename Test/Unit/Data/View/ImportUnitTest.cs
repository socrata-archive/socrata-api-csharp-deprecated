using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class ImportUnitTest : AssertionHelper
	{
		[Test]
		public void TestThrowsExceptionWithNonExistentFile()
		{
			string filename = "non_existent.csv";
			try
			{
				View v = new View();
				v.columns = new Column[0];
				v.ImportFile(filename);
				Assert.Fail("Should not be able to import non-existent file");
			}
			catch(ArgumentException)
			{}
		}
	}
}
