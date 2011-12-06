using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;

// Disable default 'value' compiler warnings, as the Row class
// is most often used as an envelope to be deserialized into,
// which populates the values through reflection
#pragma warning disable 0649

namespace Socrata.Data.View
{
	public class Row : Model<Row>
	{
		private int position;
		private string id;
		private int sid;
		
		[ScriptIgnore]
		private IDictionary<string, object> data;
		
		[ScriptIgnore]
		public IDictionary<string, object> Data {
			get {
				if (this.data == null)
				{
					this.data = new Dictionary<string, object>();
				}
				return this.data;
			}
			set {
				this.data = value;	
			}
		}
		
		[ScriptIgnore]
		public string Id {
			get {
				return this.id;
			}
		}
		
		[ScriptIgnore]
		public int Position {
			get {
				return this.position;
			}
		}
		
		[ScriptIgnore]
		public int Sid {
			get {
				return this.sid;
			}
		}
		
		[ScriptIgnore]
		public View Parent;

		public override string ResourceId()
		{
			return id;
		}

		public override string EndpointBase()
		{
			if (this.Parent == null)
				throw new InvalidOperationException("Cannot create a row without first setting a parent");

			return String.Format("{0}/{1}", this.Parent.ResourceUrl(), base.EndpointBase());
		}
		
		public void SetValue(Column col, object data)
		{
			this.Data.Add(col.id.ToString(), data);
		}
		
		// So that when we serialize, we send row data, not metadata
		protected override object getSelf()
		{
			return data;
		}
	}
}
