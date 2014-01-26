using System;
using System.Collections.Generic;
using MongoDB.Bson;
using ServiceStack.ServiceHost;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GeoAPI
{
	[Route ("/place/create", "POST")]
	public class PlaceCreateRequest
	{
		public string name { get; set; }

		public double latitude { get; set; }

		public double longitude { get; set; }

		public int radius { get; set; }
	}

	[Route ("/place/update/{Id}", "PUT")]
	public class PlaceUpdateRequest
	{
		[BsonId]
		[ApiMember (Name = "Id", Description = "Place ID", ParameterType = "path", DataType = "string", IsRequired = true)]
		public ObjectId Id { get; set; }

		public string name { get; set; }

		public double latitude { get; set; }

		public double longitude { get; set; }

		public int radius { get; set; }
	}

	[Route ("/place/list", "GET")]
	public class PlaceListRequest
	{
	}

	[Route ("/place/info/{Id}", "GET")]
	public class PlaceRequest
	{
		[ApiMember (Name = "Id", Description = "Place ID", 
			ParameterType = "path", DataType = "string", IsRequired = true)]
		public ObjectId Id { get; set; }
	}

	[Route ("/place/delete/{Id}", "DELETE")]
	public class PlaceDeleteRequest
	{
		[BsonId]
		[ApiMember (Name = "Id", Description = "Place ID", 
			ParameterType = "path", DataType = "string", IsRequired = true)]
		public ObjectId Id { get; set; }
	}
}

