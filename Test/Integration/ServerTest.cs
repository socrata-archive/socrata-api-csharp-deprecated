using System;
using NUnit.Framework;
using Socrata;

namespace SocrataTests
{
	[TestFixture]
	public class ServerTest
	{
		[Test]
		public void TestConnectivity ()
		{
			Socrata.Server.IConnection conn = TestHelper.GetConnection();
			conn.Url = "version";
			conn.ReadJsonResponse();
		}
		
		[Test]
		[ExpectedException(typeof(Socrata.Exceptions.NotFoundException))]
		public void TestInvalidUrlThrowsNotFound ()
		{
			Socrata.Server.IConnection conn = TestHelper.GetConnection();
			conn.Url = "four_oh_four.json";
			conn.ReadJsonResponse();
		}
	}
}