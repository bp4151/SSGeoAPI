using System;
using ServiceStack.ServiceInterface.ServiceModel;

namespace GeoAPI
{
	public class PushByPlaceIDResponse : IHasResponseStatus
	{
		public int MessageCount { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

	public class PushResponse : IHasResponseStatus
	{
		public ResponseStatus ResponseStatus {
			get;
			set;
		}
	}
}

