using System;
using MongoDB.Bson;
using System.Collections.Generic;
using ServiceStack.ServiceInterface.ServiceModel;

namespace GeoAPI
{
	public class Trigger
	{
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
	//trigger/list
	public class TriggerListResponse : IHasResponseStatus
	{
		public List<Trigger> triggers { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}
	//trigger/id:
	//trigger/create
	//trigger/update
	public class TriggerResponse : IHasResponseStatus
	{
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

		public ResponseStatus ResponseStatus { get; set; }
	}
	//trigger/delete
	public class TriggerDeleteResponse : IHasResponseStatus
	{
		public ResponseStatus ResponseStatus { get; set; }
	}
	//trigger/run/id:
	public class TriggerRunResponse : IHasResponseStatus
	{
		public ResponseStatus ResponseStatus { get; set; }
	}
}

