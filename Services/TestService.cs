using System;
using ServiceStack.ServiceInterface;
using System.Collections.Generic;

namespace GeoAPI
{
	public class TestService : Service
	{
		public TestListResponse Get (TestListRequest request)
		{
			TestListResponse response = new TestListResponse ();
			response.Tests = new List<TestResponse> ();
			response.Tests.Add (new TestResponse { ID = 1, Test = "Test1" });
			return response;
		}
	}
}

