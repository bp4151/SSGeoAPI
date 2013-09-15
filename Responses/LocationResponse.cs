using System;
using ServiceStack.ServiceInterface.ServiceModel;

namespace GeoAPI
{
	public class LocationResponse
	{
		public string TriggerType { get; set; }

		public ResponseStatus responseStatus { get; set; }
	}
}

