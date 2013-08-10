using System;
using MongoDB.Bson;
using System.Collections.Generic;
using ServiceStack.ServiceInterface.ServiceModel;

namespace GeoAPI
{
	//trigger/list
	public class TriggerListResponse
	{
		public List<TriggerResponse> triggers { get; set; }

		public ResponseStatus responseStatus { get; set; }
	}
	//trigger/id:
	//trigger/create
	//trigger/update
	public class TriggerResponse
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

		public ResponseStatus responseStatus { get; set; }
	}
	//trigger/delete
	public class TriggerDeleteResponse
	{
		public ResponseStatus responseStatus { get; set; }
	}
	//trigger/run/id:
	public class TriggerRunResponse
	{
		public int result { get; set; }

		public string type { get; set; }

		public string message { get; set; }

		public ResponseStatus responseStatus { get; set; }
	}
}

