using System;
using System.Net;
using Socrata.Data;

namespace Socrata
{
	public class SocrataWebClient : WebClient
	{
		public SocrataWebClient() : base()
		{
			this.Credentials = Configuration.Credentials;
			this.Headers.Add("Authorization", Configuration.Authorization);
			this.Headers.Add("X-App-Token", Configuration.AppToken);
		}
	}
}
