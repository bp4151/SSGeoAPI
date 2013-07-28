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

namespace GeoAPI
{
	public class PlaceService : Service
	{
		AppSettings appSettings = new AppSettings ();
		private MongoClient client = null;
		private MongoServer server = null;
		MongoDatabase db = null;
		MongoCollection<PlaceResponse> placescollection = null;
		MongoCollection<Trigger> triggerscollection = null;

		public PlaceService ()
		{
			var connectionString = appSettings.Get ("MongoDB", "");
			client = new MongoClient (connectionString);
			server = client.GetServer ();
			db = server.GetDatabase ("geoapi");
			placescollection = db.GetCollection<PlaceResponse> ("place");
			triggerscollection = db.GetCollection<Trigger> ("trigger");
			if (!BsonClassMap.IsClassMapRegistered (typeof(PlaceResponse))) {
				BsonClassMap.RegisterClassMap<PlaceResponse> ();
			}
		}

		/// <summary>
		/// place/list
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceListResponse Get (PlaceListRequest request)
		{
			PlaceListResponse response = new PlaceListResponse ();

			response.places = new List<PlaceResponse> ();
			var places = placescollection.FindAll ();

			try {
				foreach (var place in places) {
					Console.WriteLine (JsonSerializer.SerializeToString (place));
					PlaceResponse placeresponse = new PlaceResponse ();
					placeresponse.Id = place.Id;
					placeresponse.loc = new GeoJson2DGeographicCoordinates (place.loc.Longitude, place.loc.Latitude);
					placeresponse.name = place.name;
					placeresponse.radius = place.radius;
					placeresponse.usersInPlace = place.usersInPlace;
					response.places.Add (placeresponse);
				}
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}

			response.responseStatus = new ResponseStatus ();
			response.responseStatus.ErrorCode = "200";
			response.responseStatus.Message = "SUCCESS";
			return response;
		
		}

		/// <summary>
		/// place/info
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceResponse Get (PlaceRequest request)
		{
			PlaceResponse response = new PlaceResponse ();

			var query = Query.EQ ("_id", request.Id);
			Place result = placescollection.FindOneAs<Place> (query);

			response.Id = result.Id;
			response.loc = result.loc;
			response.name = result.name;
			response.radius = result.radius;
			response.usersInPlace = result.usersInPlace;

			response.responseStatus = new ResponseStatus ();
			response.responseStatus.ErrorCode = "200";
			response.responseStatus.Message = "SUCCESS";
			return response;

		}

		/// <summary>
		/// place/create
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceCreateResponse Post (PlaceCreateUpdateRequest request)
		{
			PlaceCreateResponse response1 = new PlaceCreateResponse ();
			PlaceResponse response2 = new PlaceResponse ();

			response2.loc = new GeoJson2DGeographicCoordinates (request.longitude, request.latitude);
			response2.name = request.name;
			response2.radius = request.radius;

			WriteConcernResult result = placescollection.Insert (response2);

			response1.Id = response2.Id;
			response1.responseStatus = new ResponseStatus ();

			if (result.Ok) {
				response1.responseStatus.ErrorCode = "200";
				response1.responseStatus.Message = "SUCCESS";
			} else {
				response1.responseStatus.ErrorCode = "500";
				response1.responseStatus.Message = "FAILURE";
			}

			return response1;
		}

		/// <summary>
		/// place/update
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceUpdateResponse Put (PlaceCreateUpdateRequest request)
		{
			PlaceUpdateResponse response = new PlaceUpdateResponse ();

			PlaceResponse place = new PlaceResponse ();
			place.Id = request.Id;
			GeoJson2DGeographicCoordinates loc = new GeoJson2DGeographicCoordinates (request.longitude, request.latitude);
			place.loc = loc;
			place.name = request.name;
			place.radius = request.radius;

			var query = Query.EQ ("_id", request.Id);
			var update = Update.Replace (place);
			FindAndModifyResult result = placescollection.FindAndModify (query, SortBy.Null, update);

			response.Id = request.Id;
			response.loc = loc;
			response.name = request.name;
			response.radius = request.radius;
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

		/// <summary>
		/// place/delete
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceDeleteResponse Delete (PlaceDeleteRequest request)
		{
			PlaceDeleteResponse response = new PlaceDeleteResponse ();

			///Remove triggers on this place before deleting the place!
			var query1 = Query.EQ ("_id", request.Id);
			PlaceResponse place = placescollection.FindOne (query1);
			FindAndModifyResult result1 = placescollection.FindAndRemove (query1, SortBy.Null);

			var query2 = Query.EQ ("placeID", place.Id);
			FindAndModifyResult result2 = triggerscollection.FindAndRemove (query2, SortBy.Null);

			response.responseStatus = new ResponseStatus ();

			if (result1.Ok && result2.Ok) {
				response.responseStatus.ErrorCode = "200";
				response.responseStatus.Message = "SUCCESS";
			} else {
				response.responseStatus.ErrorCode = "500";
				response.responseStatus.Message = "FAILURE";
			}
			return response;
		}
	}
}

