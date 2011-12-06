using System;
using System.Collections.Specialized;
using System.Net;

namespace Socrata.Exceptions
{
	public class SocrataServerException : ApplicationException
	{
		public String ErrorMessage { get { return response.Headers.Get (MessageField); } }
		public String Code         { get { return response.Headers.Get (ErrorField); } }
		public Uri    Location     { get { return response.ResponseUri; } }

		public WebException CausedBy { get { return causedBy; } }

		protected WebException causedBy;
		private HttpWebResponse response;
		
		public SocrataServerException(WebException ex)
		{
			causedBy = ex;
			if (ex.Response != null)
				response = (HttpWebResponse) ex.Response;
		}
		
		public static Exception Parse(WebException ex)
		{
			if (ex.Status == WebExceptionStatus.ConnectFailure || ex.Response == null)
			{
				// We don't have any information to parse, so just throw the original exception
				return ex;
			}
			switch (((HttpWebResponse)ex.Response).StatusCode)
			{
				case HttpStatusCode.NotFound:
					return new NotFoundException(ex);
				case HttpStatusCode.Forbidden:
					return new ForbiddenException(ex);
				case HttpStatusCode.BadRequest:
					return new InvalidRequestException(ex);

				default:
					return new SocrataServerException(ex);
			}
		}
		
		public override string ToString()
		{
			return String.Format("{0} ({1}) <{2}>", ErrorMessage, Code, (Location != null ? Location.ToString() : "none"));
		}
		
		private static string ErrorField   = "X-Error-Code";
		private static string MessageField = "X-Error-Message";
	}
}

