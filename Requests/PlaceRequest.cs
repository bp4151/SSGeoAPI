using System;
using System.Collections.Generic;
using MongoDB.Bson;
using ServiceStack.ServiceHost;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GeoAPI
{
	[Route("/place/create/", "POST")]
	[Route("/place/update/{Id}", "PUT")]
	public class PlaceCreateUpdateRequest
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public string name { get; set; }

		public GeoJson2DGeographicCoordinates loc { get; set; }

		public int radius { get; set; }
	}

	[Route("/place/list", "GET")]
	public class PlaceListRequest
	{
	}

	[Route("/place/info/{Id}", "GET")]
	public class PlaceRequest
	{
	}

	[Route("/place/delete/{Id}", "POST")]
	public class PlaceDeleteRequest
	{
		[BsonId]
		public ObjectId Id { get; set; }
	}
}

