using System;
using ServiceStack.ServiceInterface.ServiceModel;
using System.Collections.Generic;

namespace GeoAPI
{
	public class TestResponse : IHasResponseStatus
	{
		public int ID { get; set; }

		public string Test { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

	public class TestListResponse : IHasResponseStatus
	{
		public List<TestResponse> Tests { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}
}

