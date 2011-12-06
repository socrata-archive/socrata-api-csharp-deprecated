using System;
using System.Net;
using System.Configuration;

namespace Socrata.Data
{
	public class Configuration
	{
		private static ICredentials _credentials;
		
		public static int TicketCheckDelay
		{
			get
			{ 
				return 10000; // 10 seconds 
			}
		}
		
		public static ICredentials Credentials
		{
			get
			{
				if (_credentials == null)
				{
					_credentials = new NetworkCredential(
						ConfigurationManager.AppSettings["socrata.username"],
						ConfigurationManager.AppSettings["socrata.password"]);
				}
				return _credentials;
			}
		}
		
		public static string Authorization
		{
			get
			{
	            string creds = String.Format("{0}:{1}", 
							ConfigurationManager.AppSettings["socrata.username"],
							ConfigurationManager.AppSettings["socrata.password"]);
	            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(creds);
	            string base64 = Convert.ToBase64String(bytes);
	            return "Basic " + base64;
			}
		}
		
		public static string RemoteHost
		{
			get
			{
				return ConfigurationManager.AppSettings["socrata.host"];
			}
		}
		
		public static string AppToken
		{
			get
			{
				return ConfigurationManager.AppSettings["socrata.app_token"];
			}
		}
		
		private Configuration ()
		{
		}
	}
}

