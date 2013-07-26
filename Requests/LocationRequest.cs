using System;
using ServiceStack.ServiceHost;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GeoAPI
{
	public class LocationRequestList
	{
		public List<LocationRequest> locations { get; set; }
	}

	public class LocationRequest
	{
		public ObjectId Id { get; set; }

		public DateTime create_date { get; set; }

		public string user_id { get; set; }

		public Location location { get; set; }
	}
	//Private Classes
	public class Location
	{
		public Position position { get; set; }

		public String type { get; set; }
	}

	public class Position
	{

		public double latitude { get; set; }

		public double longitude { get; set; }

		public int speed { get; set; }

		public int altitude { get; set; }

		public int horizontal_accuracy { get; set; }

		public int vertical_accuracy { get; set; }
	}
}

