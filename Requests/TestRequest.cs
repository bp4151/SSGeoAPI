using System;
using ServiceStack.ServiceHost;

namespace GeoAPI
{
	[Route("/Test/", "GET")]
	public class TestListRequest
	{
	}

	[Route("/Test/{id}", "GET")]
	public class TestRequest
	{
		public int id { get; set; }
	}
}

