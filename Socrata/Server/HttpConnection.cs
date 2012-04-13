using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Socrata.Exceptions;
using Socrata.Data;

namespace Socrata.Server
{
	public class HttpConnection : IConnection
	{
		private HttpWebRequest _request;
		public string Url { get; set; }
		public IDictionary<string, string> Parameters { get { return _parameters; } }
		
		private IDictionary<string, string> _parameters = new Dictionary<string, string>();
		private Boolean _prepared = false;
		private string _method;
		
		public string Method
		{
			get
			{ return _method; }
			set
			{ _method = value; }
		}
		
		public Stream OutboundStream
		{
			get
			{
				prepare();
				return _request.GetRequestStream();
			}
		}
		
		public HttpWebResponse Response
		{
			get
			{
				prepare();
				return (HttpWebResponse) _request.GetResponse();	
			}
		}
		
		public Stream InboundStream
		{
			get
			{
				prepare();
				try
				{
					return _request.GetResponse().GetResponseStream();
				}
				catch (WebException ex)
				{
					throw SocrataServerException.Parse(ex);
				}
			}
		}
		
		public string ReadJsonResponse()
		{
			using (var reader = new StreamReader(this.InboundStream))
			{
                return reader.ReadToEnd();
            }
		}
		
		public void Close()
		{
			prepare();
			if (_request.GetResponse() == null)
				OutboundStream.Close();
			_request.GetResponse().Close();
		}
			
		public String GetQueryParameters()
		{
			var queryParts =
				from entry in _parameters
				select String.Format("{0}={1}", entry.Key, entry.Value);
			return String.Join("&", queryParts.ToArray());
		}
				
		private void prepare()
		{
			if (_prepared)
				return;
	
			if (_parameters.Count > 0)
			{
				// TODO: More robust version
				Url += (Url.IndexOf("?") < 0) ? "?" : "&";
				Url += GetQueryParameters();
			}
			_request = (HttpWebRequest) WebRequest.Create(
				String.Format("{0}/api/{1}",
				Configuration.RemoteHost, Url));
			_request.Credentials = Configuration.Credentials;
			_request.PreAuthenticate = true;
				
			_request.Headers["X-App-Token"] = Configuration.AppToken;
            _request.Headers["Authorization"] = Configuration.Authorization;
			
			_request.Method = (_method == null) ? "GET" : _method;
			
			_prepared = true;
		}
	}
}
