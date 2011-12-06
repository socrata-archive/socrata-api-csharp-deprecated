using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;

using Socrata.Server;

namespace Socrata.Data.View
{
	/// <summary>
	/// A view is the model behind both datasets and filtered views
	/// </summary>
	public class View : Model<View>
	{

		public string name { get; set; }
		public string id { get; set; }
		
		public string displayType { get; set; }
		public IList<Column> columns { get; set; }
		public string category { get; set; }
		public string description { get; set; }
		public string attribution { get; set; }
		public string attributionLink { get; set; }
		
		public Query query { get; set; }
		public IDictionary<string, object> metadata { get; set; }
		public bool publicationAppendEnabled { get; set; }
		
		public IList<string> flags { get; set; }
		public IList<Grant> grants { get; set; }
		public IList<string> tags { get; set; }
		public string publicationStage { get; set; }
		
		private Boolean _columnsMapped = false;
		
		public PublicationStage Stage()
		{
			if (this.publicationStage == null)
			{
				return PublicationStage.Unpublished;
			}
			return (PublicationStage) Enum.Parse (typeof(PublicationStage), Strings.TitleCase(publicationStage));	
		}
		
		public bool CanAddRows()
		{
			return publicationAppendEnabled || Stage() == PublicationStage.Unpublished;	
		}
		
		public bool CanAppendOrReplace()
		{
			return Stage() == PublicationStage.Unpublished;
		}
		
		public IList<Column> Columns()
		{
			if (columns == null)
			{
				if (this.ResourceId() != null)
				{
					columns = Column.DeserializeList(getConnectionForResource("/columns"));
				}
				else
				{
					columns = new List<Column>();
				}
			}
			if (!_columnsMapped)
			{
				foreach(var column in columns)
				{
					column.Parent = this;
				}
				_columnsMapped = true;
			}
			return columns;
		}
		
		public IList<string> Topics()
		{
			if (this.tags == null)
			{
				this.tags = new List<string>();	
			}
			return this.tags;
		}
		
		// The mono serializer inexplicably chokes when
		// this method is called Grants. So give it a
		// less-succint name :/
		public IList<Grant> ViewGrants()
		{
			if (this.grants == null)
			{
				this.grants = new List<Grant>();
			}
			return this.grants;
		}
		
		public string DisplayType()
		{
			if (this.displayType == null)
			{
				return "";
			}
			return this.displayType;
		}
		
		/// <summary>
		/// Deletes the specified column.
		/// </summary>
		/// <param name='column'>
		/// Column to delete.
		/// </param>
		public void DeleteColumn(Column column)
		{
			if (column == null || column.ResourceId() == null)
				throw new ArgumentNullException("Need a column with a valid ID to delete");

			column.Parent = this;
			column.Delete();
		}
		
		/// <summary>
		/// Adds a column to the view and creates it on the server.
		/// </summary>
		/// <param name='column'>
		/// The column to add.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
		/// </exception>
		public Column AddColumn(Column column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("Need a column to create");
			}
			column.Parent = this;
			// Can't create a column on the server if the view isn't created yet
			if (this.ResourceId() != null)
			{
				// pre-fetch existing columns if not already there
				// to avoid chicken and egg problem
				this.Columns();
				column = column.Create();
				// Re-set the parent as it's not serialized back
				column.Parent = this;
			}
			this.Columns().Add(column);
			return column;
		}
		
		/// <summary>
		/// Adds the row to the dataset.
		/// </summary>
		/// <returns>
		/// The row.
		/// </returns>
		/// <param name='row'>
		/// Row.
		/// </param>
		public Row AddRow(Row row)
		{
			checkCanAdd();
			row.Parent = this;
			return row.Create();
		}
		
		private void checkCanAdd()
		{
			if (!CanAppendOrReplace())
			{
				throw new InvalidOperationException("Can only append or replace unpublished views");	
			}
		}
		
		/// <summary>
		/// Makes the view public or private.
		/// </summary>
		/// <param name='isPublic'>
		/// Whether or not to set to public (default true).
		/// </param>
		public void SetPublic(bool isPublic = true)
		{
			string permission = isPublic ? "public.read" : "private";
			IConnection conn = getConnectionForResource(String.Format("?method=setPermission&value={0}", permission), "PUT");
			conn.Close();
		}
		
		/// <summary>
		/// Determines whether this view is public.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this view is public; otherwise, <c>false</c>.
		/// </returns>
		public bool IsPublic()
		{
			foreach(var grant in this.ViewGrants())
			{
				if (grant.flags != null && grant.flags.Count > 0)
				{
					if (grant.flags.Contains("public") &&
						((this.DisplayType().Equals("form") && grant.type != null && grant.type.Equals("contributor")) ||
						   !this.DisplayType().Equals("form")))
					{
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Grabs the view object for a known UID
		/// </summary>
		/// <returns>
		/// The view object
		/// </returns>
		/// <param name='id'>
		/// The four-four UID of the view to use
		/// </param>
		public static View FromId(String id)
		{
			View view = new View();
			view.id = id;
			return Deserialize(view.getConnectionForResource());
		}
		
		/// <summary>
		/// Publish this view.
		/// </summary>
		public View Publish()
		{
			if (this.Stage() != PublicationStage.Unpublished)
			{
				throw new InvalidOperationException("Can only publish unpublished views");
			}
			
			int pending = pendingGeocodingRequests();
			while (pending > 0)
			{
				System.Threading.Thread.Sleep(Configuration.TicketCheckDelay);
				pending = pendingGeocodingRequests();
			}
			
			IConnection publishingConn = getConnectionForResource(PublicationEndpoint, "POST");
			return Deserialize(publishingConn);
		}
		
		/// <summary>
		/// Copies the dataset synchonously, ignoring all row data.
		/// </summary>
		/// <returns>
		/// A new unpublished view with the same schema. Useful for replacing.
		/// </returns>
		public View CopySchema()
		{
			var publishingConn = getConnectionForResource(PublicationEndpoint, "POST");
			publishingConn.Parameters.Add("method", "copySchema");
			// publishingConn.Parameters.Add("viewId", this.ResourceId());
			
			return Deserialize(publishingConn);
		}
		
		/// <summary>
		/// Creates a working copy of the dataset, allowing you to make changes.
		/// </summary>
		/// <returns>
		/// The working copy.
		/// </returns>
		public View WorkingCopy()
		{
			var publishingConn = getConnectionForResource(PublicationEndpoint, "POST");
			publishingConn.Method = "POST";
			publishingConn.Parameters.Add("method", "copy");
			publishingConn.Parameters.Add("viewId", ResourceId());
			
			// Wait while the server is still processing
			while (publishingConn.Response.StatusCode == HttpStatusCode.Accepted)
			{
				System.Threading.Thread.Sleep(Configuration.TicketCheckDelay);
				publishingConn.Close();

				publishingConn = getConnectionForResource(PublicationEndpoint);
				publishingConn.Parameters.Add("method", "copy");
			}

			return Deserialize(publishingConn);
		}
		
		/// <summary>
		/// Imports the file.
		/// </summary>
		/// <returns>
		/// A view with the data imported.
		/// </returns>
		/// <param name='fileName'>
		/// The path to the file to import.
		/// </param>
		/// <param name='skip'>
		/// How many rows to skip.
		/// </param>
		/// <param name='translation'>
		/// An optional translation. See publisher documentation for details.
		/// </param>
		public View ImportFile(string fileName, int skip = 0, string translation = "")
		{	
			var parameters = new Dictionary<string, string>();
			parameters.Add("translation", translation);
			return importinate(fileName, parameters, skip);
		}
		
		public View Append(string fileName, int skip = 0, string translation = "")
		{
			return appendOrReplace(fileName, skip, translation);
		}
		
		public View Replace(string fileName, int skip = 0, string translation = "")
		{
			return appendOrReplace(fileName, skip, translation, false);
		}
		
		private View appendOrReplace(string fileName, int skip, string translation, bool append = true)
		{
			checkCanAdd();
			
			var parameters = new Dictionary<string, string>();
			parameters.Add("method", append ? "append" : "replace");
			parameters.Add("translation", translation);
			parameters.Add("viewUid", ResourceId());
			parameters.Add("skip", skip.ToString());
			
			return importinate(fileName, parameters);
		}
		
		public IDictionary<string, object> GetMetadata()
		{
			if (this.metadata == null)
			{
				this.metadata = new Dictionary<string, object>();	
			}
			return this.metadata;
		}

        public void SetMetadata(IDictionary<string, object> meta)
        {
            this.metadata = meta;
        }
		
		[ScriptIgnore]
		public IDictionary<string, object> CustomFields
		{
			get
			{
				if (!this.GetMetadata().ContainsKey(CustomFieldsId))
				{
					this.GetMetadata()[CustomFieldsId] = new Dictionary<string, object>();
				}
				return (IDictionary<string, object>)this.GetMetadata()[CustomFieldsId];
			}
			set
			{
				this.GetMetadata()[CustomFieldsId] = value;
			}	
		}
		
		public IDictionary<string, object> MetadataFieldset(string fieldsetName)
		{
			if (!CustomFields.ContainsKey(fieldsetName))
			{
				throw new ArgumentException("No such fieldset exists on this view");
			}
			
			return (IDictionary<string, object>) CustomFields[fieldsetName];
		}
		
		public IList<IDictionary<string, object>> GetRows(int start, int length)
		{
			var conn = getConnectionForResource("/rows", "POST");
			conn.Parameters.Add ("method", "getRows");
			conn.Parameters.Add ("start", start.ToString());
			conn.Parameters.Add ("length", length.ToString());
			return Model<IDictionary<string, object>>.DeserializeList(conn);
		}
		
		private View importinate(string fileName, IDictionary<string, string> parameters, int skip = 0)
		{
			FileInfo info = new FileInfo(fileName);
			if (!info.Exists)
			{
				throw new ArgumentException("Can't find a file with that path");
			}

			var report = scan(fileName);
			
			// Use scan column suggestions if we're importing a new file
			// and no columns are set
			if (this.ResourceId() == null && this.Columns().Count == 0)
			{
				foreach(var col in report.summary.TranslatedColumns())
				{
					this.Columns().Add(col);
				}
			}
			
			if (this.ResourceId() == null && !parameters.ContainsKey("blueprint"))
			{
				parameters.Add("blueprint", GetSerializer().Serialize(blueprint(skip)));	
			}

			var conn = ConnectionFactory.CreateConnection();
			conn.Url = ImportsEndpoint;
			conn.Method = "POST";
			conn.Parameters.Add("fileId", report.fileId);
			
			foreach(var entry in parameters)
			{
				conn.Parameters.Add(entry.Key, entry.Value);
			}
			
			if (!conn.Parameters.ContainsKey("name"))
			{
				conn.Parameters.Add("name", info.Name);	
			}
			
			// Wait while the server is still processing
			while (conn.Response.StatusCode == HttpStatusCode.Accepted)
			{
				System.Threading.Thread.Sleep(Configuration.TicketCheckDelay);
				var response = DeserializeObject(conn);
				conn.Close();
				
				conn = ConnectionFactory.CreateConnection();
				conn.Url = ImportsEndpoint;
				conn.Method = "GET";
				conn.Parameters.Add("ticket", (string)response["ticket"]);
			}
			return Deserialize(conn);
		}
		
		private int pendingGeocodingRequests()
		{
			var conn = ConnectionFactory.CreateConnection();
			conn.Parameters.Add("method", "pending");
			
			conn.Url = String.Format("geocoding/{0}", this.ResourceId());
			var response = DeserializeObject(conn);
			return (int)response["view"];
		}
		
		private ScanResult scan(string fileName)
		{
			return UploadFile<ScanResult>(String.Format("{0}?method=scan", ImportsEndpoint), fileName);
		}
		
		public Blueprint blueprint(int skip)
		{
			var blue = new Blueprint();
			blue.name = this.name;
			blue.description = this.description;
			blue.columns = blueprintColumns();
			blue.skip = skip;
			return blue;
		}
		
		private List<BlueprintColumn> blueprintColumns()
		{
			var cols = new List<BlueprintColumn>();
			foreach(Column viewCol in this.Columns())
			{
				var bpCol = new BlueprintColumn();
				bpCol.name = viewCol.name;
				bpCol.description = viewCol.description;
				bpCol.datatype = viewCol.dataTypeName;
				cols.Add(bpCol);
			}
			return cols;
		}
		
		public override string ResourceId()
		{
			return id;
		}
		
		public enum PublicationStage
		{
			Copying,
			Unpublished,
			Snapshotted,
			Published
		}
		
		public class Grant
		{
			public IList<string> flags;
			public bool inherited;
			public string type;
		}
		
		// Note: the '/' before publication is necessary, because this is
		// getting added after a view id
		private static string PublicationEndpoint = "/publication";
		private static string ImportsEndpoint     = "imports2";
		private static string CustomFieldsId      = "custom_fields";
	}
}
