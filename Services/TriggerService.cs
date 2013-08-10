using System;
using ServiceStack.ServiceInterface;
using ServiceStack.Configuration;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;
using ServiceStack.Text;
using ServiceStack.ServiceInterface.ServiceModel;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace GeoAPI
{
	public class TriggerService : Service
	{
		AppSettings appSettings = new AppSettings ();
		private MongoClient client = null;
		private MongoServer server = null;
		MongoDatabase db = null;
		MongoCollection<TriggerResponse> triggerscollection = null;

		public TriggerService ()
		{
			var connectionString = appSettings.Get ("MongoDB", "");
			client = new MongoClient (connectionString);
			server = client.GetServer ();
			db = server.GetDatabase ("geoapi");
			//Get locations
			triggerscollection = db.GetCollection<TriggerResponse> ("trigger");

			if (!BsonClassMap.IsClassMapRegistered (typeof(TriggerResponse))) {
				BsonClassMap.RegisterClassMap<TriggerResponse> ();
			}

		}
		//List
		public TriggerListResponse Get (TriggerListRequest request)
		{
			TriggerListResponse response = new TriggerListResponse ();

			response.triggers = new List<TriggerResponse> ();
			var triggers = triggerscollection.FindAll ();

			try {

				foreach (var trigger in triggers) {
					Console.WriteLine (JsonSerializer.SerializeToString (trigger));
					TriggerResponse triggerresponse = new TriggerResponse ();
					triggerresponse.Id = trigger.Id;
					triggerresponse.placeId = trigger.placeId;
					triggerresponse.dateFrom = trigger.dateFrom;
					triggerresponse.dateTo = trigger.dateTo;
					triggerresponse.delay = trigger.delay;
					triggerresponse.extra = trigger.extra;
					triggerresponse.Id = trigger.Id;
					triggerresponse.perUserRunCount = trigger.perUserRunCount;
					triggerresponse.text = trigger.text;
					triggerresponse.timeFrom = trigger.timeFrom;
					triggerresponse.timeTo = trigger.timeTo;
					triggerresponse.type = trigger.type;
					response.triggers.Add (triggerresponse);
				}
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}

			response.responseStatus = new ResponseStatus ();
			response.responseStatus.ErrorCode = "200";
			response.responseStatus.Message = "SUCCESS";

			return response;
		}
		//Info
		public TriggerResponse Get (TriggerRequest request)
		{
			TriggerResponse response = new TriggerResponse ();

			var query = Query.EQ ("_id", request.Id);
			var trigger = triggerscollection.FindOneAs<TriggerResponse> (query);
			response.dateFrom = trigger.dateFrom;
			response.dateTo = trigger.dateTo;
			response.delay = trigger.delay;
			response.extra = trigger.extra;
			response.Id = trigger.Id;
			response.perUserRunCount = trigger.perUserRunCount;
			response.text = trigger.text;
			response.timeFrom = trigger.timeFrom;
			response.timeTo = trigger.timeTo;
			response.type = trigger.type;
			
			response.responseStatus = new ResponseStatus ();
			response.responseStatus.ErrorCode = "200";
			response.responseStatus.Message = "SUCCESS";

			return response;
		}
		//Create
		public TriggerResponse Post (TriggerRequest request)
		{
			TriggerResponse response = new TriggerResponse ();

			response.dateFrom = request.dateFrom;
			response.dateTo = request.dateTo;
			response.delay = request.delay;
			response.extra = request.extra;
			response.perUserRunCount = request.perUserRunCount;
			response.placeId = request.placeId;
			response.text = request.text;
			response.timeFrom = request.timeFrom;
			response.timeTo = request.timeTo;
			response.type = request.type;

			WriteConcernResult result = triggerscollection.Insert (response);

			response.responseStatus = new ResponseStatus ();

			if (result.Ok) {
				response.responseStatus.ErrorCode = "200";
				response.responseStatus.Message = "SUCCESS";
			} else {
				response.responseStatus.ErrorCode = "500";
				response.responseStatus.Message = "FAILURE";
			}

			return response;

		}
		//Update
		public TriggerResponse Put (TriggerRequest request)
		{
			TriggerResponse response = new TriggerResponse ();

			var query = Query.EQ ("_id", new BsonObjectId (request.Id));

			//var trigger = triggerscollection.FindOneAs<TriggerResponse> (query);

			response.Id = request.Id;
			response.placeId = request.placeId;
			response.dateFrom = request.dateFrom;
			response.dateTo = request.dateTo;
			response.delay = request.delay;
			response.extra = request.extra;
			response.perUserRunCount = request.perUserRunCount;
			response.text = request.text;
			response.timeFrom = request.timeFrom;
			response.timeTo = request.timeTo;
			response.type = request.type;

			var update = Update.Replace (response);
//			var update = Update
//				.Set ("placeId", request.placeId)
//				.Set ("dateFrom", request.dateFrom)
//				.Set ("dateTo", request.dateTo)
//				.Set ("delay", request.delay)
//		      	.SetWrapped ("extra", request.extra)
//				.Set ("perUserRunCount", request.perUserRunCount)
//				.Set ("text", request.text)
//				.Set ("timeFrom", request.timeFrom)
//				.Set ("timeTo", request.timeTo)
//				.Set ("type", request.type);
			FindAndModifyResult result = triggerscollection.FindAndModify (query, SortBy.Null, update, true);

			response.responseStatus = new ResponseStatus ();

			if (result.Ok) {
				response.responseStatus.ErrorCode = "200";
				response.responseStatus.Message = "SUCCESS";
			} else {
				response.responseStatus.ErrorCode = "500";
				response.responseStatus.Message = "FAILURE";
			}

			return response;

		}
		//Delete
		public TriggerDeleteResponse Delete (TriggerDeleteRequest request)
		{
			TriggerDeleteResponse response = new TriggerDeleteResponse ();

			var query = Query.EQ ("_id", request.Id);

			FindAndModifyResult result = triggerscollection.FindAndRemove (query, SortBy.Null);

			response.responseStatus = new ResponseStatus ();

			if (result.Ok) {
				response.responseStatus.ErrorCode = "200";
				response.responseStatus.Message = "SUCCESS";
			} else {
				response.responseStatus.ErrorCode = "500";
				response.responseStatus.Message = "FAILURE";
			}

			return response;

		}
		//Run
	}
}

