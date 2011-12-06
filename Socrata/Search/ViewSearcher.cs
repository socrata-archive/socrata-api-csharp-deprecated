using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Web.Script.Serialization;

using Socrata.Data;
using Socrata.Data.View;
using Socrata.Server;

// Disable default 'value' compiler warnings, as the ViewSearchResult class
// is simply an envelope to be deserialized into, and is never instantiated
// by the programmer
#pragma warning disable 0649

namespace Socrata.Search
{
	public class ViewSearcher
	{
		/// <summary>
		/// Performs a search for views matching the passed parameters 
		/// </summary>
		/// <returns>
		/// A list of views matching the query parameters.
		/// </returns>
		/// <param name='query'>
		/// A textual query
		/// </param>
		/// <param name='rowCount'>
		/// If you'd like to see rows as well, pass a maximum of 50 here
		/// to get up to that many matching rows.
		/// </param>
		public static List<ViewSearchResult> PerformSearch(string query, int? rowCount = null, IDictionary<string, string> extraParams = null)
		{
			if (extraParams == null)
			{
				extraParams = new Dictionary<string, string>();	
			}
			
			if (rowCount != null)
			{
				extraParams["rowCount"] = rowCount.ToString();
			}
			
			extraParams["q"] = query;
			return PerformSearch (extraParams);
		}
		
		public static List<ViewSearchResult> PerformSearch(IDictionary<string, string> searchParams)
		{
			IConnection conn = ConnectionFactory.CreateConnection();
			conn.Url = "search/views";
			
			foreach(string key in searchParams.Keys)
			{
				conn.Parameters.Add(key, searchParams[key]);
			}
			
			return SearchEnvelope.Deserialize(conn).results;
		}
		
		class SearchEnvelope : Model<SearchEnvelope>
		{
			public List<ViewSearchResult> results;
			public int count;
			
			public override string ResourceId() { return null; }
		}
	}
	
	

	public class ViewSearchResult
	{
		// The view that matches the query
		public View view;
		// If you pass a row count parameter to the search
		// and the query matches row data, you'll get rows here
		public List<object> rows;
		// If you searched for rows and the search yielded
		// more rows than you specified in row count, this is the
		// number of total matching rows
		public int totalRows;
	}
}

