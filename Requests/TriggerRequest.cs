using System;
using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.ServiceHost;

namespace GeoAPI
{
	/*

	trigger/list
	{}
	
	trigger/list/place_id:
	{
		
	}

    trigger/info/id:
	{
		_id:
	}

	trigger/create
	{
	        place_id:
	        text:
	        extra:
	        date_from:
	        date_to:
	        time_from:
	        time_to:
	        type:(ENTER, EXIT)
	        per_user_count: (1 or 0)
	        delay: (seconds)
	}

	trigger/delete/:id
	{}

	trigger/run/:place_id
	{
	        place_id:
	}
	 */
	[Route("/trigger/list/", "GET")]
	public class TriggerListRequest
	{
	}

	[Route("/trigger/list/{placeId}", "GET")]
	public class TriggerListByPlaceRequest
	{
		public ObjectId placeId { get; set; }
	}

	[Route("/trigger/info/{Id}", "GET")]
	public class TriggerRequest
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public string text { get; set; }

		public Dictionary<string, object> extra { get; set; }

		public DateTime dateFrom { get; set; }

		public DateTime dateTo  { get; set; }

		public DateTime timeFrom { get; set; }

		public DateTime timeTo { get; set; }

		public string type { get; set; }
		//(ENTER, EXIT)
		public int perUserRunCount { get; set; }
		//1 or more, 0 unlimited
		public int delay { get; set; }
		//(seconds)
	}

	[Route("/trigger/", "POST")]
	public class TriggerCreateRequest
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public string text { get; set; }

		public Dictionary<string, object> extra { get; set; }

		public DateTime dateFrom { get; set; }

		public DateTime dateTo  { get; set; }

		public DateTime timeFrom { get; set; }

		public DateTime timeTo { get; set; }

		public string type { get; set; }
		//(ENTER, EXIT)
		public int perUserRunCount { get; set; }
		//1 or more, 0 unlimited
		public int delay { get; set; }
		//(seconds)
	}

	[Route("/trigger/{Id}", "DELETE")]
	public class TriggerDeleteRequest
	{
		public ObjectId Id { get; set; }
	}

	[Route("/trigger/run/{placeId}", "POST")]
	public class TriggerRunRequest
	{
		public ObjectId placeId { get; set; }
	}
}

