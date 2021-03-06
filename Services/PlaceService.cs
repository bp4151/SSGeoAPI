using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ServiceStack.Configuration;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Linq;

namespace GeoAPI
{
	public class PlaceService : Service
	{
		AppSettings appSettings = new AppSettings ();
		private MongoClient client = null;
		private MongoServer server = null;
		MongoDatabase db = null;
		MongoCollection<Place> placescollection = null;
		MongoCollection<Trigger> triggerscollection = null;

		public PlaceService ()
		{
			var connectionString = appSettings.Get ("MongoDB", "");
			client = new MongoClient (connectionString);
			server = client.GetServer ();
			db = server.GetDatabase ("geoapi");
			placescollection = db.GetCollection<Place> ("place");
			triggerscollection = db.GetCollection<Trigger> ("trigger");
			if (!BsonClassMap.IsClassMapRegistered (typeof(Place))) {
				BsonClassMap.RegisterClassMap<Place> ();
			}
			if (!BsonClassMap.IsClassMapRegistered (typeof(Trigger))) {
				BsonClassMap.RegisterClassMap<Trigger> ();
			}

		}

		/// <summary>
		/// place/list
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceListResponse Get (PlaceListRequest request)
		{
			PlaceListResponse response = new PlaceListResponse ();
			response.responseStatus = new ResponseStatus ();

			response.places = new List<Place> ();
			var places = placescollection.FindAll ();

			try {
				foreach (var place in places) {
					//Console.WriteLine (JsonSerializer.SerializeToString (place));
					Place _place = new Place ();
					_place.usersInPlace = new List<string> ();

					_place.Id = place.Id;
					_place.loc = new GeoJson2DGeographicCoordinates (place.loc.Longitude, place.loc.Latitude);
					_place.name = place.name;
					_place.radius = place.radius;
					_place.usersInPlace = place.usersInPlace;
					_place.createDate = place.createDate;

					response.places.Add (_place);
				}
				response.responseStatus.ErrorCode = "200";
				response.responseStatus.Message = "SUCCESS";

			} catch (Exception ex) {
				response.responseStatus.ErrorCode = "500";
				response.responseStatus.Message = ex.Message;
				response.responseStatus.StackTrace = ex.StackTrace;
			}


			return response;
		
		}

		/// <summary>
		/// place/info
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceResponse Get (PlaceRequest request)
		{
			PlaceResponse response = new PlaceResponse ();
			response.usersInPlace = new List<string> ();

			var query = Query.EQ ("_id", request.Id);
			Place result = placescollection.FindOneAs<Place> (query);

			response.Id = result.Id;
			response.loc = result.loc;
			response.name = result.name;
			response.radius = result.radius;
			response.usersInPlace = result.usersInPlace;

			response.ResponseStatus = new ResponseStatus ();
			response.ResponseStatus.ErrorCode = "200";
			response.ResponseStatus.Message = "SUCCESS";
			return response;

		}

		/// <summary>
		/// place/create
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceCreateResponse Post (PlaceCreateRequest request)
		{
			PlaceCreateResponse response = new PlaceCreateResponse ();
			Place place = new Place ();

			try {
				place.loc = new GeoJson2DGeographicCoordinates (request.longitude, request.latitude);
				place.name = request.name;
				place.radius = request.radius;
				place.createDate = DateTime.Now;
				place.usersInPlace = GetUsersInPlace (request);

				WriteConcernResult result = placescollection.Insert (place);

				response.Id = place.Id;
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
				response.ResponseStatus.StackTrace = ex.StackTrace;
				return response;
			}
		}

		/// <summary>
		/// place/update
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceUpdateResponse Put (PlaceUpdateRequest request)
		{
			PlaceUpdateResponse response = new PlaceUpdateResponse ();

			// Convert to PlaceCreateRequest so we can get the new list of usersInPlace
			PlaceCreateRequest pcr = new PlaceCreateRequest ();
			pcr.latitude = request.latitude;
			pcr.longitude = request.longitude;
			pcr.name = request.name;
			pcr.radius = request.radius;

			//Create the new place object so we can update it
			Place place = new Place ();
			place.Id = request.Id;
			GeoJson2DGeographicCoordinates loc = new GeoJson2DGeographicCoordinates (request.longitude, request.latitude);
			place.loc = loc;
			place.name = request.name;
			place.radius = request.radius;
			//Get the users in place based on the new data
			place.usersInPlace = GetUsersInPlace (pcr);

			var query = Query.EQ ("_id", request.Id);
			var update = Update.Replace (place);

			FindAndModifyArgs args = new FindAndModifyArgs {
				Query = query,
				SortBy = SortBy.Null,
				Update = update
			};

			//FindAndModifyResult result = placescollection.FindAndModify (query, SortBy.Null, update);
			FindAndModifyResult result = placescollection.FindAndModify (args);

			response.Id = request.Id;
			response.loc = loc;
			response.name = request.name;
			response.radius = request.radius;
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

		/// <summary>
		/// place/delete
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceDeleteResponse Delete (PlaceDeleteRequest request)
		{
			PlaceDeleteResponse response = new PlaceDeleteResponse ();

			///Remove triggers on this place before deleting the place!

			var triggerQuery = Query.EQ ("placeId", request.Id);
			FindAndRemoveArgs triggerArgs = new FindAndRemoveArgs ();
			triggerArgs.Query = triggerQuery;
			triggerArgs.SortBy = SortBy.Null;
			FindAndModifyResult triggerResult = triggerscollection.FindAndRemove (triggerArgs);
			 
			var placeQuery = Query.EQ ("_id", request.Id);
			FindAndRemoveArgs placeArgs = new FindAndRemoveArgs ();
			placeArgs.Query = placeQuery;
			placeArgs.SortBy = SortBy.Null;
			FindAndModifyResult placeResult = placescollection.FindAndRemove (placeArgs);

			response.ResponseStatus = new ResponseStatus ();

			if (placeResult.Ok && triggerResult.Ok) {
				response.ResponseStatus.ErrorCode = "200";
				response.ResponseStatus.Message = "SUCCESS";
			} else {
				response.ResponseStatus.ErrorCode = "500";
				response.ResponseStatus.Message = "FAILURE";
			}
			return response;
		}

		private List<string> GetUsersInPlace (PlaceCreateRequest request)
		{
			List<string> usersInPlace = new List<string> ();

			var uom = appSettings.GetString ("UoM") ?? "";

			var earthRadius = 0; //63780; // m

			try {

				switch (uom) {
				case "METER":
					{
						earthRadius = 63710;
						break;
					}
				case "KILOMETER":
					{
						earthRadius = 6371;
						break;
					}
				case "MILE":
					{
						earthRadius = 3959;
						break;
					}
				default:
					{
						earthRadius = 63710;
						break;
					}
				}

				//Get locations
				MongoCollection<Location> locationscollection = db.GetCollection<Location> ("location");

				if (!BsonClassMap.IsClassMapRegistered (typeof(Location))) {
					BsonClassMap.RegisterClassMap<Location> ();
				}

				float radius = (float)request.radius / (float)earthRadius;

				//Set the query to determine if the selected location is in the place
				var icquery = Query.WithinCircle ("loc", request.longitude, request.latitude, radius, true);

				//Debug
				Console.WriteLine (icquery);

				//Get all locations inside place for user where createdate is less than a day old and the record is the most recent 
				List<Location> listlocsinsideplace = locationscollection.Find (icquery)
				.Where (l => l.create_date > DateTime.Now.AddDays (-1) && l.order_col == 0)
					.ToList ();

				usersInPlace = listlocsinsideplace.Select (l => l.user_id).Distinct ().ToList ();

				return usersInPlace;
			} catch (Exception ex) {
				throw ex;
			}
		}
	}
}

