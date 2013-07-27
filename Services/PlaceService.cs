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

namespace GeoAPI
{
	public class PlaceService : Service
	{
		AppSettings appSettings = new AppSettings ();

		/// <summary>
		/// place/list
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceListResponse Get (PlaceListRequest request)
		{
			PlaceListResponse response = new PlaceListResponse ();

			var connectionString = appSettings.Get ("MongoDB", "");
			MongoClient client = new MongoClient (connectionString);
			MongoServer server = client.GetServer ();

			MongoDatabase db = server.GetDatabase ("geoapi");
			MongoCollection<PlaceResponse> placescollection = db.GetCollection<PlaceResponse> ("place");

			if (!BsonClassMap.IsClassMapRegistered (typeof(PlaceResponse))) {
				BsonClassMap.RegisterClassMap<PlaceResponse> ();
			}

			response.places = new List<PlaceResponse> ();

			try {
				foreach (var place in placescollection.FindAll ()) {
					Console.WriteLine (JsonSerializer.SerializeToString (place));
					response.places.Add (place);
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


			return response;
		}

		/// <summary>
		/// place/create
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceCreateResponse Post (PlaceCreateUpdateRequest request)
		{
			PlaceCreateResponse response = new PlaceCreateResponse ();

			var connectionString = appSettings.Get ("MongoDB", "");
			MongoClient client = new MongoClient (connectionString);
			MongoServer server = client.GetServer ();

			MongoDatabase db = server.GetDatabase ("geoapi");
			MongoCollection<PlaceResponse> placescollection = db.GetCollection<PlaceResponse> ("place");
			if (!BsonClassMap.IsClassMapRegistered (typeof(PlaceResponse))) {
				BsonClassMap.RegisterClassMap<PlaceResponse> ();
			}

			WriteConcernResult result = placescollection.Insert (request);

			response.Id = request.Id;
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
		/// place/update
		/// </summary>
		/// <param name="request">Request.</param>
		public PlaceUpdateResponse Put (PlaceCreateUpdateRequest request)
		{
			PlaceUpdateResponse response = new PlaceUpdateResponse ();

			var connectionString = appSettings.Get ("MongoDB", "");
			MongoClient client = new MongoClient (connectionString);
			MongoServer server = client.GetServer ();

			MongoDatabase db = server.GetDatabase ("geoapi");
			MongoCollection<PlaceResponse> placescollection = db.GetCollection<PlaceResponse> ("place");
			if (!BsonClassMap.IsClassMapRegistered (typeof(PlaceResponse))) {
				BsonClassMap.RegisterClassMap<PlaceResponse> ();
			}

			var query = Query.EQ ("_id", request.Id);
			var update = Update.Set ("loc", BsonValue.Create (request.loc)).Set ("name", BsonValue.Create (request.name)).Set ("radius", BsonValue.Create (request.radius));

			FindAndModifyResult result = placescollection.FindAndModify (query, SortBy.Null, update);

			response.Id = request.Id;
			response.loc = request.loc;
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


			return response;
		}
	}
}

