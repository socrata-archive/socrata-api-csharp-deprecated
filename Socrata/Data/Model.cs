using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Socrata.Server;

namespace Socrata.Data
{
	public abstract class Model<T>
	{
		public abstract string ResourceId();
		
		protected IConnection CreateConnection(String url, String method = null)
		{
			IConnection conn = ConnectionFactory.CreateConnection();
			conn.Url = url;
			conn.Method = method;
			
			return conn;
		}
		
		public virtual string EndpointBase()
		{
			return typeof(T).Name.ToLower() + "s";
		}
		
		public string EndpointBaseWithParent(string parentBase)
		{
			return parentBase + "/" + EndpointBase();
		}
		
		public virtual string ResourceUrl()
		{
			return String.Format("{0}/{1}", EndpointBase(), ResourceId());
		}
		
		public void Update()
		{
			IConnection conn = CreateConnection(ResourceUrl(), "PUT");
			WriteSelfToConnection(conn);
			conn.InboundStream.Close();
		}
		
		public T Create()
		{
			IConnection conn = CreateConnection(EndpointBase(), "POST");
			WriteSelfToConnection(conn);
			return Deserialize(conn);
		}
		
		public void Delete()
		{
			IConnection conn = CreateConnection(ResourceUrl(), "DELETE");
			conn.InboundStream.Close();
		}
		
		protected void WriteSelfToConnection(IConnection conn)
		{
			var ser = GetSerializer();
			StreamWriter sw = new StreamWriter(conn.OutboundStream);
			sw.Write(ser.Serialize(getSelf()));
			sw.Close();
		}

		protected virtual object getSelf()
		{
			return this;
		}
		
		public static T Deserialize(IConnection conn)
		{
			return GetSerializer().Deserialize<T>(conn.ReadJsonResponse());
		}
		
		public static IDictionary<string, object> DeserializeObject(IConnection conn)
		{
			return GetSerializer().Deserialize<Dictionary<string, object>>(conn.ReadJsonResponse());	
		}
		
		public static IList<T> DeserializeList(IConnection conn)
		{
			return GetSerializer().Deserialize<List<T>>(conn.ReadJsonResponse());
		}
		
		protected static JavaScriptSerializer GetSerializer()
		{
			return new JavaScriptSerializer();
		}
		
		protected static IDictionary<string, object> UploadFile(string url, string fileName)
		{
			return UploadFile<IDictionary<string, object>>(url, fileName);	
		}
		
		protected static U UploadFile<U>(string url, string fileName)
		{
			WebClient client = ConnectionFactory.CreateWebClient();
			var endpoint = String.Format("{0}/api/{1}", Configuration.RemoteHost, url);

			byte[] responseBytes = client.UploadFile(endpoint, fileName);
			string response = System.Text.Encoding.UTF8.GetString(responseBytes);
			return GetSerializer().Deserialize<U>(response);
		}
		
		protected IConnection getConnectionForResource(string resource = null, string method = null)
		{
			return CreateConnection(String.Format("{0}{1}", ResourceUrl(),(resource == null ? "" : resource)), method);
		}
	}
}