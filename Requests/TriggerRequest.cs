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

	[Route("/trigger/create", "POST")]
	public class TriggerCreateRequest
	{
		public ObjectId placeId { get; set; }

		public string text { get; set; }

		public Dictionary<string, object> extra { get; set; }

		public DateTime dateFrom { get; set; }

		public DateTime dateTo  { get; set; }

		public DateTime timeFrom { get; set; }

		public DateTime timeTo { get; set; }
		//(ENTER, EXIT)
		public string type { get; set; }
		//1 or more, 0 unlimited
		public int perUserRunCount { get; set; }
		//(seconds)
		public int delay { get; set; }
	}

	[Route("/trigger/update/{Id}", "PUT")]
	public class TriggerUpdateRequest
	{
		[BsonId]
		[ApiMember(Name="Id", Description = "Place ID", 
		           ParameterType = "path", DataType = "string", IsRequired = true)]
		public ObjectId Id { get; set; }

		public ObjectId placeId { get; set; }

		public string text { get; set; }

		public Dictionary<string, object> extra { get; set; }

		public DateTime dateFrom { get; set; }

		public DateTime dateTo  { get; set; }

		public DateTime timeFrom { get; set; }

		public DateTime timeTo { get; set; }
		//(ENTER, EXIT)
		public string type { get; set; }
		//1 or more, 0 unlimited
		public int perUserRunCount { get; set; }
		//(seconds)
		public int delay { get; set; }
	}

	[Route("/trigger/delete/{Id}", "DELETE")]
	[Route("/trigger/info/{Id}", "GET")]
	public class TriggerRequest
	{
		[BsonId]
		[ApiMember(Name="Id", Description = "Place ID", 
		           ParameterType = "path", DataType = "string", IsRequired = true)]
		public ObjectId Id { get; set; }
	}

	[Route("/trigger/run/{Id}", "POST")]
	public class TriggerRunRequest
	{
		[BsonId]
		[ApiMember(Name="Id", Description = "Trigger ID", 
		           ParameterType = "path", DataType = "string", IsRequired = true)]
		public ObjectId Id { get; set; }
		/*
		[ApiMember(Name="device_platform", Description = "Device Platform", 
		           ParameterType = "body", DataType = "string", IsRequired = true)]
		[ApiAllowableValues("device_platform", new string[] {"ios", "android"})] //Enum
		*/
		public string device_platform { get; set; }
	}
}

