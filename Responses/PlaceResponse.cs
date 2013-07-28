using System;
using System.Collections.Generic;
using MongoDB.Bson;
using ServiceStack.ServiceInterface.ServiceModel;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GeoAPI
{
	public class PlaceResponse
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public string name { get; set; }

		public GeoJson2DGeographicCoordinates loc { get; set; }

		public int radius { get; set; }

		public List<string> usersInPlace { get; set; }

		public ResponseStatus responseStatus { get; set; }
	}

	public class PlaceCreateResponse
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public ResponseStatus responseStatus { get; set; }
	}

	public class PlaceUpdateResponse
	{
		[BsonId]
		public ObjectId? Id { get; set; }

		public string name { get; set; }

		public GeoJson2DGeographicCoordinates loc { get; set; }

		public int radius { get; set; }

		public List<string> usersInPlace { get; set; }

		public ResponseStatus responseStatus { get; set; }
	}

	public class PlaceListResponse
	{
		public List<PlaceResponse> places { get; set; }

		public ResponseStatus responseStatus { get; set; }
	}

	public class PlaceDeleteResponse
	{
		public ResponseStatus responseStatus { get; set; }
	}
}

