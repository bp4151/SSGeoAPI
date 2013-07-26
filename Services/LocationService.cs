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

namespace GeoAPI
{
	public class LocationService : Service
	{
		AppSettings appSettings = new AppSettings ();

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

			string connectionString = appSettings.Get ("MongoDB", "");
			int locationLimit = appSettings.Get ("LocationLimit", 0);
			MongoClient client = new MongoClient (connectionString);
			MongoServer server = client.GetServer ();
			MongoDatabase db = server.GetDatabase ("geoapi");
			//Get locations
			MongoCollection<LocationRequest> locationscollection = db.GetCollection<LocationRequest> ("location");

			if (!BsonClassMap.IsClassMapRegistered (typeof(LocationRequest))) {
				BsonClassMap.RegisterClassMap<LocationRequest> ();
			}

			//Insert the location into the locations collection
			locationscollection.Insert (request);

			//if we have a locationLimit from the appSettings, then each time we insert for a user, remove all but the limit from the collection;
			if (locationLimit > 0) {
				var query = Query<LocationRequest>.EQ (l => l.user_id, request.user_id);
				var entities = locationscollection.Find (query).OrderByDescending (l => l.Id).Skip (locationLimit);
				foreach (var entity in entities) {
					locationscollection.Remove (Query.EQ ("_id", entity.Id));
				}

			}

			//Store found placeIDs and trigger types in a dictionary
			Dictionary<ObjectId, string> dictPlaces = GetPlacesByLocation (request);

			//For each entry in the dictionary, 
			//if ENTER, then run enter trigger where placeID = placeID and type = ENTER
			//if EXIT, then run exit trigger where placeID = placeID and type = EXIT

			foreach (DictionaryEntry de in dictPlaces) {
				RunTrigger (de.Key, de.Value);
			}

			LocationResponse response = new LocationResponse ();
			response.responseStatus = new ResponseStatus ();
			response.responseStatus.ErrorCode = "200";
			response.responseStatus.Message = "SUCCESS";
			return response;
		}

		Dictionary<ObjectId, string> GetPlacesByLocation (LocationRequest request)
		{

			string connectionString = appSettings.Get ("MongoDB", "");
			MongoClient client = new MongoClient (connectionString);
			MongoServer server = client.GetServer ();
			MongoDatabase db = server.GetDatabase ("geoapi");

			var earthRadius = 6378.0; // km

			Dictionary<ObjectId, string> dictPlaces = new Dictionary<ObjectId, string> ();

			//Get places
			MongoCollection<PlaceResponse> placescollection = db.GetCollection<PlaceResponse> ("place");
			MongoCollection<LocationRequest> locationscollection = db.GetCollection<LocationRequest> ("location");

			placescollection.EnsureIndex (IndexKeys.GeoSpatialSpherical ("Loc"));

			foreach (PlaceResponse place in placescollection.FindAll ()) {
				var icquery = Query.WithinCircle ("Loc", place.loc.Longitude, place.loc.Latitude, place.radius / earthRadius, true);

				//Get all locations for user by createdate desc
				var locs = locationscollection.FindAll ().Where (u => u.user_id == request.user_id).OrderByDescending (l => l.create_date);
				//Get all locations inside place for user by createdate desc
				var locsinsideplace = locationscollection.Find (icquery).Where (u => u.user_id == request.user_id).OrderByDescending (l => l.create_date);

				List<Location> listlocs = locs.ToList ();
				List<Location> listlocsinsideplace = locsinsideplace.ToList ();

				//If the list of locations inside the place has a count of 1, then the user is new to the place
				if (listlocsinsideplace.Count == 1) {
					dictPlaces.Add (place.Id, "ENTER");
					//If an enter trigger exists on the place, fire the trigger and add the user to the place
					Console.WriteLine ("Execute ENTER trigger");
				}
				//If the list of locations inside the place has a count > 1, and the id of the location inside the place is the most current, and the second location is not inside the place, then the user has entered the place
				else if (listlocsinsideplace.Count > 1 && listlocs [0].Id == listlocsinsideplace [0].Id && listlocs [1].Id == listlocsinsideplace [0].Id) {
					dictPlaces.Add (place.Id, "ENTER");
					//If an enter trigger exists on the place, fire the trigger and add the user to the place
					Console.WriteLine ("Execute ENTER trigger");
				}
				//If the list of locations inside the place has a count > 1, and the id of the location inside the place is the second location, and the first location is not inside the place, then the user has left the place
				else if (listlocsinsideplace.Count > 1 && listlocs [1].Id == listlocsinsideplace [0].Id && listlocs [0].Id != listlocsinsideplace [0].Id) {
					dictPlaces.Add (place.Id, "EXIT");
					//If an exit trigger exists on the place, fire the trigger and remove the user to the place
					Console.WriteLine ("Execute EXIT trigger");
				} else {
					Console.WriteLine ("Still in place");
				}

			}

			return dictPlaces;

		}

		void RunTrigger (ObjectId place_id, string triggerType)
		{
			string connectionString = appSettings.Get ("MongoDB", "");
			MongoClient client = new MongoClient (connectionString);
			MongoServer server = client.GetServer ();
			MongoDatabase db = server.GetDatabase ("geoapi");

		}
	}
}

