using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Socrata.Data;

namespace Socrata.Server
{
	public interface IConnection
	{
		string Url { get; set; }
		string Method { get; set; }
		HttpWebResponse Response { get; }
		IDictionary<string, string> Parameters { get; }
		
		/// <summary>
		/// Gets the outbound stream, used to send data as the body of a request.
		/// </summary>
		Stream OutboundStream { get; }
		/// <summary>
		/// Gets the inbound stream, to receive response data.
		/// </summary>
		/// <value>
		/// The inbound stream.
		/// </value>
		Stream InboundStream { get; }
		/// <summary>
		/// Reads the response stream, constructing a string ready to be deserialized from JSON.
		/// </summary>
		/// <returns>
		/// The a stringified JSON representation of the response.
		/// </returns>
		string ReadJsonResponse();
		/// <summary>
		/// Closes the request.
		/// </summary>
		void Close();
	}
}
