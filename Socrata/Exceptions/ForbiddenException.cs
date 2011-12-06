using System;
using System.Net;

namespace Socrata.Exceptions
{
	public class ForbiddenException : SocrataServerException
	{
		public ForbiddenException(WebException ex) : base(ex)
		{
		}
	}
}

