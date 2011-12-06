using System;
using System.Collections.Generic;

namespace Socrata.Data.View
{
	public class BlueprintColumn
	{
		public string name		  { get; set; }
		public string description { get; set; }
		public string datatype    { get; set; }
	}
	
	public class Blueprint
	{		
		public int skip { get; set; }
		public string name;
		public string description { get; set; }
		public IList<BlueprintColumn> columns { get; set; }	
		
		public Blueprint ()
		{
		}
	}
}
