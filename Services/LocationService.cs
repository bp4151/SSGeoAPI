using System;
using System.Collections.Generic;
using System.Collections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ServiceStack.Configuration;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;
using MongoDB.Driver.Builders;
using System.Linq;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Text;

namespace GeoAPI
{
	public class LocationService : Service
	{
		AppSettings appSettings = new AppSettings ();
		private int locationLimit = 0;
		private MongoClient client = null;
		private MongoServer server = null;
		MongoDatabase db = null;
		MongoCollection<Location> locationscollection = null;

		public LocationService ()
		{
			var connectionString = appSettings.Get ("MongoDB", "");
			locationLimit = appSettings.Get ("LocationLimit", 0);
			client = new MongoClient (connectionString);
			server = client.GetServer ();
			db = server.GetDatabase ("geoapi");
			//Get locations
//			locationscollection = db.GetCollection<LocationRequest> ("location");
//
//			if (!BsonClassMap.IsClassMapRegistered (typeof(LocationRequest))) {
//				BsonClassMap.RegisterClassMap<LocationRequest> ();
//			}
			locationscollection = db.GetCollection<Location> ("location");

//			var cdkeys = IndexKeys.Descending ("create_date");
//			var cdoptions = IndexOptions.SetName ("location.create_date");
//
//			locationscollection.CreateIndex (cdkeys, cdoptions);
//
//			var ukeys = IndexKeys.Ascending ("user_id");
//			var uoptions = IndexOptions.SetName ("location.user_id");
//
//			locationscollection.CreateIndex (ukeys, uoptions);
//
//			var ikeys = IndexKeys.Ascending ("order_col");
//			var ioptions = IndexOptions.SetName ("location.order_col");
//
//			locationscollection.CreateIndex (ikeys, ioptions);

			var ikeys = IndexKeys.Ascending ("user_id", "order_col");
			var ioptions = IndexOptions.SetName ("location.user_id_order_col");

			locationscollection.CreateIndex (ikeys, ioptions);

			if (!BsonClassMap.IsClassMapRegistered (typeof(Location))) {
				BsonClassMap.RegisterClassMap<Location> ();
			}
		}

		/// <summary>
		/// Post the user's location.
		/// 
		/// When a new location is uploaded, we need to
		/// 1) Determine if that location is within the radius of a place in the places collection
		/// 2) If it is inside a place, add it to the place.usersInPlace array if it is not already there
		/// 3) If there is an entry trigger, and the user's previous location does not exist or was outside the place, run the entry trigger
		/// 4) If it was inside a place, and it is now outside of the place, remove it from the place.usersInPlace array if it is there
		/// 5) If there is an exit trigger, and the user's previous location exists and was inside the place, run the exit trigger
		/// </summary>
		/// <param name="request">LocationRequest</param>
		public LocationResponse Post (LocationRequest request)
		{
			LocationResponse response = new LocationResponse ();
			response.responseStatus = new ResponseStatus ();

			try {

				//***************************************************************************************
				//Create new location object and set variables for insert into location collection
				Location location = new Location ();
				location.user_id = request.user_id;
				if (request.create_date == DateTime.MinValue) {
					location.create_date = DateTime.Now;
				} else {
					location.create_date = request.create_date;
				}

				location.loc = new GeoJson2DGeographicCoordinates (request.longitude, request.latitude);
				//***************************************************************************************

				//***************************************************************************************
				//Find all records for the user whose location is being updated, and update all their 
				//records incrementing the order_col + 1
				var query = Query.EQ ("user_id", request.user_id);
				var sortBy = SortBy.Null;
				var update = Update.Inc ("order_col", 1.0);
				WriteConcernResult updresult = locationscollection.Update (query, update, UpdateFlags.Multi);
				//***************************************************************************************

				//***************************************************************************************
				//Insert the location into the locations collection
				locationscollection.Insert (location);
				//***************************************************************************************

				//***************************************************************************************
				//if we have a locationLimit from the appSettings, then each time we insert for a user, 
				//remove all but the limit from the collection;
				if (locationLimit > 0) {

					var delquery = Query.And (
						Query.EQ ("user_id", request.user_id),
						Query.GTE ("order_col", locationLimit)
					);

					WriteConcernResult delresult = locationscollection.Remove (delquery);

//					var entities = locationscollection.Find (delquery);
//
//					foreach (var entity in entities) {
//						locationscollection.Remove (Query.EQ ("_id", entity.Id));
//					}

				}
				//***************************************************************************************

				//***************************************************************************************
				//For this location, check all places to see if this location is:
				//1) Now in the place but the previous location wasn't (ENTER TRIGGER)
				//2) Not in the place but the previous location was (EXIT TRIGGER)
				//3) Now in the place and the previous location was as well (DWELLING)
				//4) Not in the place and the previous location was not as welll (NOT IN PLACE)
				GetPlacesByLocation (request);
				//***************************************************************************************


				response.responseStatus.ErrorCode = "200";
				response.responseStatus.Message = "SUCCESS";
				return response;
			} catch (Exception ex) {
				response.responseStatus.ErrorCode = "500";
				response.responseStatus.Message = ex.Message;
				response.responseStatus.StackTrace = ex.StackTrace;
				return response;
			}
		}

		/// <summary>
		/// Gets the places by location.
		/// </summary>
		/// <param name="request">Request.</param>
		void GetPlacesByLocation (LocationRequest request)
		{
			var earthRadius = 6378.0; // km
		
			try {
					
				//Get places
				MongoCollection<PlaceResponse> placescollection = db.GetCollection<PlaceResponse> ("place");
				MongoCollection<Location> locationscollection = db.GetCollection<Location> ("location");

				if (!BsonClassMap.IsClassMapRegistered (typeof(PlaceResponse))) {
					BsonClassMap.RegisterClassMap<PlaceResponse> ();
				}
				if (!BsonClassMap.IsClassMapRegistered (typeof(Location))) {
					BsonClassMap.RegisterClassMap<Location> ();
				}

				placescollection.EnsureIndex (IndexKeys.GeoSpatialSpherical ("loc"));

				foreach (var place in placescollection.FindAll ()) {

					//Set the query to determine if the new location is in each place
					var icquery = Query.WithinCircle ("loc", place.loc.Longitude, place.loc.Latitude, place.radius / earthRadius, false);

					//Debug
					Console.WriteLine (icquery);

					//Get all locations for user by order_col desc
					//var locs = locationscollection.FindAll ().Where (u => u.user_id == request.user_id).OrderByDescending (l => l.order_col);
					//Get all locations inside place for user by order_col desc
					//var locsinsideplace = locationscollection.Find (icquery).Where (l => l.user_id == request.user_id).OrderByDescending (l => l.order_col);

					//List<Location> listlocs = new List<Location> ();
					//List<Location> listlocsinsideplace = new List<Location> ();

					List<Location> listlocs = locationscollection.FindAll ()
						.Where (u => u.user_id == request.user_id)
						.OrderBy (l => l.order_col)
						.ToList ();

					List<Location> listlocsinsideplace = locationscollection.Find (icquery)
						.Where (l => l.user_id == request.user_id)
						.OrderBy (l => l.order_col)
						.ToList ();

					//get the total list of locations for the given user into a list
					//listlocs = locs.ToList ();

					//get the list of locations that are in this place
					//listlocsinsideplace = locsinsideplace.ToList ();

					int idxCurrentLocation = -1;
					int idxPriorLocation = -1;

					idxCurrentLocation = listlocsinsideplace.FindIndex (l => l.Id == listlocs [0].Id);
					Console.WriteLine ("idxCurrentLocation: " + idxCurrentLocation);

					if (listlocs.Count > 1) {
						idxPriorLocation = listlocsinsideplace.FindIndex (l => l.Id == listlocs [1].Id);
						Console.WriteLine ("idxPriorLocation: " + idxPriorLocation);
					}

					//if idxCurrentLocation = -1 and idxPriorLocation = -1 location not in place
					if (idxCurrentLocation == -1 && idxPriorLocation == -1) {
						Console.WriteLine ("Not in place");
					}

					//if idxCurrentLocation = 0 and idxPriorLocation = -1 ENTER
					else if (idxCurrentLocation == 0 && idxPriorLocation == -1) {
						//dictPlaces.Add (place.Id, "ENTER");
						//If an enter trigger exists on the place, fire the trigger and add the user to the place
						place.usersInPlace.AddIfNotExists (request.user_id);
						placescollection.Save (place);
						Console.WriteLine ("Execute ENTER trigger");
						RunTrigger (place.Id, place.usersInPlace, "ENTER");
					}

					//if idxCurrentLocation = -1 and idxPriorLocation = 0 EXIT
					else if (idxCurrentLocation == -1 && idxPriorLocation == 0) {
						//dictPlaces.Add (place.Id, "EXIT");
						//If an exit trigger exists on the place, fire the trigger and remove the user to the place
						place.usersInPlace.Remove (request.user_id);
						placescollection.Save (place);
						Console.WriteLine ("Execute EXIT trigger");
						List<string> userlist = new List<string> ();
						userlist.Add (request.user_id);
						RunTrigger (place.Id, userlist, "EXIT");
					}

					//if idxCurrentLocation = 0 and idxPriorLocation >= 0 STILL IN PLACE
					else if (idxCurrentLocation == 0 && idxPriorLocation >= 0) {
						Console.WriteLine ("Still in place");
					}

				}

			} catch (Exception ex) {

			}
			//return dictPlaces;

		}

		void RunTrigger (ObjectId place_id, List<string> usersInPlace, string triggerType)
		{

			var appHost = this.GetAppHost ();
			var plugin = (ACSPushFeature)appHost.Plugins.Find (x => x is ACSPushFeature);

			string connectionString = appSettings.Get ("MongoDB", "");
			MongoClient client = new MongoClient (connectionString);
			MongoServer server = client.GetServer ();
			MongoDatabase db = server.GetDatabase ("geoapi");

			MongoCollection<TriggerResponse> triggerscollection = db.GetCollection<TriggerResponse> ("trigger");

			if (!BsonClassMap.IsClassMapRegistered (typeof(TriggerResponse))) {
				BsonClassMap.RegisterClassMap<TriggerResponse> ();
			}

			try {
				//Get all triggers for this place
				var triggerquery = Query.And (
					Query.EQ ("placeId", place_id),
					Query.EQ ("type", triggerType)
				);
				List<TriggerResponse> triggersonplace = triggerscollection.Find (triggerquery).ToList ();

				StringBuilder sb = new StringBuilder ();
				for (int i = 0; i < usersInPlace.Count; i++) {
					sb.Append (usersInPlace [0] + ",");
				}
				string userlist = sb.ToString ();
				userlist = userlist.Substring (0, userlist.Length - 1);

				for (int i = 0; i < triggersonplace.Count; i++) {
					plugin.Notify ("BAIRFINDER-DEFAULT", userlist, triggersonplace [i].text);
				}
			} catch (Exception ex) {

			}
		}
	}
}

