using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Socrata.Search;
using Socrata.Data.View;

namespace SocrataTests
{
	[TestFixture]
	public class SearchTest : TestHelper
	{
		[Test]
		public void TestPerformSearch()
		{
			// just make sure a basic search works without throwing an error
			ViewSearcher.PerformSearch("jenkins", null, getDefaultSearchParams());
		}
		
		[Test]
		public void TestSearchWithRowCount()
		{
			List<ViewSearchResult> results = ViewSearcher.PerformSearch("jenkins", 1, getDefaultSearchParams());
			if (results == null)
			{
				return;	
			}
			foreach(var vsr in results)
			{
                Expect(vsr.view, Is.InstanceOfType(typeof(View)));
				if (vsr.rows != null)
				{
                    Expect(vsr.rows.Count, Is.GreaterThanOrEqualTo(vsr.totalRows));
				}
			}
		}
		
		private IDictionary<string, string> getDefaultSearchParams()
		{
			IDictionary<string, string> defParams = new Dictionary<string, string>();
			defParams.Add("nofederate", "true");
			defParams.Add ("limit", "2");
			return defParams;
		}
	}
}

