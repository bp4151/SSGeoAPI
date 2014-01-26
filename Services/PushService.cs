using System;
using ServiceStack.ServiceInterface;
using ServiceStack.Configuration;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using ServiceStack.ServiceInterface.ServiceModel;
using MongoDB.Driver.Builders;

namespace GeoAPI
{
	public class PushService : Service
	{
		AppSettings appSettings = new AppSettings ();
		private MongoClient client = null;
		private MongoServer server = null;
		MongoDatabase db = null;
		MongoCollection<Trigger> triggerscollection = null;
		MongoCollection<Place> placecollection = null;

		public PushService ()
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

		public PushByPlaceIDResponse Post (PushByPlaceIDRequest request)
		{
			PushByPlaceIDResponse response = new PushByPlaceIDResponse ();

			response.ResponseStatus = new ResponseStatus ();

			try {

				var triggerquery = Query.EQ ("placeId", request.placeId);

				var trigger = triggerscollection.FindOneAs<Trigger> (triggerquery);

				var placequery = Query.EQ ("_id", trigger.placeId);

				var place = placecollection.FindOneAs<Place> (placequery);

				bool bResult = Utility.Push.Run (this.GetAppHost (), this.appSettings, trigger.placeId, place.usersInPlace, request.message, "");

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

