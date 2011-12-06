using System;
using System.Net;

namespace Socrata.Exceptions
{
	public class InvalidRequestException : SocrataServerException
	{
		public InvalidRequestException (WebException ex) : base(ex)
		{
		}
	}
}

