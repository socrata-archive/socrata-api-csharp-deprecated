using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace Socrata.Data.View
{
	public class Column : Model<Column>
	{
		public int id;
		public int position;
		public int tableColumnId;
		public string name;
		public string description;
		public string renderTypeName;
		public string fieldName;
		public string dataTypeName;
		public List<string> flags { get; set; }
		
		public Column()
		{
			flags = new List<string>();
		}
		
		public string GetFieldName()
        {
			return this.fieldName;
		}
		
		public enum DataType
		{
			Text,
	        Number,
	        Calendar_date,
	        Date,
	        Photo_obsolete,
	        Money,
	        Phone,
	        Checkbox,
	        Flag,
	        Stars,
	        Percent,
	        Url,
	        Document_obsolete,
	        Tag,
	        Email,
	        Nested_table,
	        /** column type which is a combo box of values */
	        Drop_down_list,
	        Html,
			/** deprecated */
	        Meta_data, Photo, Document, Location,
	        Geospatial,
	        Dataset_link,
	        /** object, */
	        List
		};
		
		[ScriptIgnore]
		public DataType Type
		{
			get
			{
				return (DataType) Enum.Parse(typeof(DataType), Strings.TitleCase(dataTypeName));
			}
			set
			{
				dataTypeName = value.ToString().ToLower();
			}
		}
		
		[ScriptIgnore]
		public View Parent;
		
		public override string EndpointBase()
		{
			string baseEndpoint = base.EndpointBase();
			if (this.Parent != null)
			{
				return String.Format("{0}/{1}", this.Parent.ResourceUrl(), baseEndpoint);
			}
			return baseEndpoint;
		}
		
		public override string ResourceId()
		{
			return id.ToString();
		}
		
	}
}

