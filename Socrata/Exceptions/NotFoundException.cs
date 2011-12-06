using System;
using System.Net;

namespace Socrata.Exceptions
{
	public class NotFoundException : SocrataServerException
	{
		public NotFoundException(WebException ex) : base(ex)
		{
		}
	}
}

