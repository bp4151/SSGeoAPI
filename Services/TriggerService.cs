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
		MongoCollection<Trigger> triggerscollection = null;
		MongoCollection<Place> placecollection = null;

		public TriggerService ()
		{
			var connectionString = appSettings.Get ("MongoDB", "");
			client = new MongoClient (connectionString);
			server = client.GetServer ();
			db = server.GetDatabase ("geoapi");
			//Get locations
			triggerscollection = db.GetCollection<Trigger> ("trigger");
			placecollection = db.GetCollection<Place> ("place");

			if (!BsonClassMap.IsClassMapRegistered (typeof(Trigger))) {
				BsonClassMap.RegisterClassMap<Trigger> ();
			}
			if (!BsonClassMap.IsClassMapRegistered (typeof(Place))) {
				BsonClassMap.RegisterClassMap<Place> ();
			}

		}
		//List
		public TriggerListResponse Get (TriggerListRequest request)
		{
			TriggerListResponse response = new TriggerListResponse ();

			response.triggers = new List<Trigger> ();
			var triggers = triggerscollection.FindAll ();

			try {

				foreach (var trigger in triggers) {
					Console.WriteLine (JsonSerializer.SerializeToString (trigger));
					Trigger _trigger = new Trigger ();
					_trigger.Id = trigger.Id;
					_trigger.placeId = trigger.placeId;
					_trigger.dateFrom = trigger.dateFrom;
					_trigger.dateTo = trigger.dateTo;
					_trigger.delay = trigger.delay;
					_trigger.extra = trigger.extra;
					_trigger.Id = trigger.Id;
					_trigger.perUserRunCount = trigger.perUserRunCount;
					_trigger.text = trigger.text;
					_trigger.timeFrom = trigger.timeFrom;
					_trigger.timeTo = trigger.timeTo;
					_trigger.type = trigger.type;
					response.triggers.Add (_trigger);
				}
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}

			response.ResponseStatus = new ResponseStatus ();
			response.ResponseStatus.ErrorCode = "200";
			response.ResponseStatus.Message = "SUCCESS";

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
			
			response.ResponseStatus = new ResponseStatus ();
			response.ResponseStatus.ErrorCode = "200";
			response.ResponseStatus.Message = "SUCCESS";

			return response;
		}
		//Create
		public TriggerResponse Post (TriggerCreateRequest request)
		{
			TriggerResponse response = new TriggerResponse ();
			Trigger trigger = new Trigger ();

			try {

				trigger.dateFrom = request.dateFrom;
				trigger.dateTo = request.dateTo;
				trigger.delay = request.delay;
				trigger.extra = request.extra;
				trigger.perUserRunCount = request.perUserRunCount;
				trigger.placeId = request.placeId;
				trigger.text = request.text;
				trigger.timeFrom = request.timeFrom;
				trigger.timeTo = request.timeTo;
				trigger.type = request.type;

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

				WriteConcernResult result = triggerscollection.Insert (trigger);

				response.ResponseStatus = new ResponseStatus ();

				if (result.Ok) {
					response.ResponseStatus.ErrorCode = "200";
					response.ResponseStatus.Message = "SUCCESS";
				} else {
					response.ResponseStatus.ErrorCode = "500";
					response.ResponseStatus.Message = "FAILURE";
				}

				return response;
			} catch (Exception ex) {
				response.ResponseStatus.ErrorCode = "500";
				response.ResponseStatus.Message = ex.Message;
				return response;
			}
		}
		//Update
		public TriggerResponse Put (TriggerUpdateRequest request)
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

			response.ResponseStatus = new ResponseStatus ();

			if (result.Ok) {
				response.ResponseStatus.ErrorCode = "200";
				response.ResponseStatus.Message = "SUCCESS";
			} else {
				response.ResponseStatus.ErrorCode = "500";
				response.ResponseStatus.Message = "FAILURE";
			}

			return response;

		}
		//Delete
		public TriggerDeleteResponse Delete (TriggerRequest request)
		{
			TriggerDeleteResponse response = new TriggerDeleteResponse ();

			var query = Query.EQ ("_id", request.Id);

			FindAndModifyResult result = triggerscollection.FindAndRemove (query, SortBy.Null);

			response.ResponseStatus = new ResponseStatus ();

			if (result.Ok) {
				response.ResponseStatus.ErrorCode = "200";
				response.ResponseStatus.Message = "SUCCESS";
			} else {
				response.ResponseStatus.ErrorCode = "500";
				response.ResponseStatus.Message = "FAILURE";
			}

			return response;

		}
		//Run
		public TriggerRunResponse Post (TriggerRunRequest request)
		{
			TriggerRunResponse response = new TriggerRunResponse ();

			response.ResponseStatus = new ResponseStatus ();

			try {

				var triggerquery = Query.EQ ("_id", request.Id);

				var trigger = triggerscollection.FindOneAs<Trigger> (triggerquery);

				var placequery = Query.EQ ("_id", trigger.placeId);

				var place = placecollection.FindOneAs<Place> (placequery);
			 
				bool bResult = Utility.Trigger.Run (this.GetAppHost (), this.appSettings, trigger.placeId, place.usersInPlace, trigger.type);

				if (bResult == true) {
					response.ResponseStatus.ErrorCode = "200";
					response.ResponseStatus.Message = "SUCCESS";
				} else {
					response.ResponseStatus.ErrorCode = "500";
					response.ResponseStatus.Message = "FAILURE";
				}

				return response;
			} catch (Exception ex) {
				response.ResponseStatus.ErrorCode = "500";
				response.ResponseStatus.Message = ex.Message;
				return response;
			}
		}
	}
}

