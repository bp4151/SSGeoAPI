using System;
using ServiceStack.ServiceHost;
using MongoDB.Bson;

namespace GeoAPI
{
	[Route ("/push/placeId/{placeId}", "POST")]
	public class PushByPlaceIDRequest
	{
		public ObjectId placeId { get; set; }

		public string message { get; set; }
	}

	[Route ("/Push", "POST")]
	public class PushRequest
	{
		public string message { get; set; }
	}
}

