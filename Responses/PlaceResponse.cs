using System;
using System.Collections.Generic;
using MongoDB.Bson;
using ServiceStack.ServiceInterface.ServiceModel;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GeoAPI
{
	public class Place
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public string name { get; set; }

		public GeoJson2DGeographicCoordinates loc { get; set; }

		public int radius { get; set; }

		public List<string> usersInPlace { get; set; }

		public DateTime createDate { get; set; }
	}

	public class PlaceResponse
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public string name { get; set; }

		public GeoJson2DGeographicCoordinates loc { get; set; }

		public int radius { get; set; }

		public List<string> usersInPlace { get; set; }

		public DateTime createDate { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

	public class PlaceCreateResponse
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

	public class PlaceUpdateResponse
	{
		[BsonId]
		public ObjectId? Id { get; set; }

		public string name { get; set; }

		public GeoJson2DGeographicCoordinates loc { get; set; }

		public int radius { get; set; }

		public List<string> usersInPlace { get; set; }

		public DateTime createDate { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

	public class PlaceListResponse
	{
		public List<Place> places { get; set; }

		public ResponseStatus responseStatus { get; set; }
	}

	public class PlaceDeleteResponse
	{
		public ResponseStatus ResponseStatus { get; set; }
	}
}

