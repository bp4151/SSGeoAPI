using System;
using ServiceStack.ServiceInterface.ServiceModel;
using System.Collections.Generic;

namespace GeoAPI
{
	public class LocationResponse
	{
		public List<String> TriggerResults { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}
}

