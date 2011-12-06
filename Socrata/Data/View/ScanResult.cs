using System;
using System.Collections.Generic;

namespace Socrata.Data.View
{
	public class ScanResult : Model<ScanResult>
	{
		public class ScanSummary
		{
			public IList<ScanColumn> columns;
			public int headers;
			public IList<IDictionary<string, object>> locations;

			
			public IList<Column> TranslatedColumns()
			{
				if (this.columns == null)
				{
					return null;
				}

				var mapped = new List<Column>();
				foreach(var col in this.columns)
				{
					Column proxy = new Column();
					proxy.dataTypeName = col.suggestion;
					proxy.name = col.name;
					mapped.Add(proxy);
				}
				
				if (this.locations != null)
				{
					foreach(var suggestion in this.locations)
					{
						if (suggestion.ContainsKey("address"))
						{
							mapped[(int)suggestion["address"]].Type = Column.DataType.Location;
						}
					}
				}
				
				return mapped;
			}
			
			public class ScanColumn
			{
				public string name;
				public int processed;
				public string suggestion;
				public IDictionary<string, int> types;
			}
		}
		
		public ScanSummary summary;
		public string fileId { get; set; }
		
		public override string ResourceId()
		{
			throw new NotSupportedException("Should not be used");
		}
	}
}
