using System;
using System.Net;

namespace Socrata.Server
{
	public class ConnectionFactory
	{
		/// <summary>
		/// Creates a connection to talk to Socrata servers.
		/// </summary>
		/// <returns>
		/// A new connection.
		/// </returns>
		public static IConnection CreateConnection()
		{
			return new HttpConnection();
		}
		
		/// <summary>
		/// Creates a web client. WebClients are used for multipart POSTS,
		/// e.g. uploading a file.
		/// </summary>
		/// <returns>
		/// A new web client.
		/// </returns>
		public static WebClient CreateWebClient()
		{
			return new SocrataWebClient();
		}
		
		private ConnectionFactory()
		{
		}
	}
}
