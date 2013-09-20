using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;
using System.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Configuration;
using System.Linq;

namespace GeoAPI.Utility
{
	public static class Trigger
	{

		public static bool Run (IAppHost appHost, AppSettings appSettings, ObjectId place_id, List<string> usersInPlace, string triggerType)
		{

			string connectionString = appSettings.Get ("MongoDB", "");
			string pushPlatform = appSettings.Get ("PushPlatform", "");
			MongoClient client = new MongoClient (connectionString);
			MongoServer server = client.GetServer ();
			MongoDatabase db = server.GetDatabase ("geoapi");

			MongoCollection<GeoAPI.Trigger> triggerscollection = db.GetCollection<GeoAPI.Trigger> ("trigger");

			if (!BsonClassMap.IsClassMapRegistered (typeof(GeoAPI.Trigger))) {
				BsonClassMap.RegisterClassMap<GeoAPI.Trigger> ();
			}

			List<GeoAPI.Trigger> triggersonplace = new List<GeoAPI.Trigger> ();

			try {

				//no longer using plugins for push. Using repository pattern instead.
				//var plugin = (ACSPushFeature)appHost.Plugins.Find (x => x is ACSPushFeature);
				//var plugin = (EverlivePushFeature)appHost.Plugins.Find (x => x is EverlivePushFeature);

				var pushfeature = appHost.GetContainer ().ResolveNamed<IPush> (pushPlatform);			

				//Get all triggers for this place
				var triggerquery = Query.And (
					Query.EQ ("placeId", place_id),
					Query.EQ ("type", triggerType)
				);
				triggersonplace = triggerscollection.Find (triggerquery).ToList ();

				if (triggersonplace.Count > 0) {
					StringBuilder sb = new StringBuilder ();
					for (int i = 0; i < usersInPlace.Count; i++) {
						sb.Append (usersInPlace [i] + ",");
					}
					string userlist = sb.ToString ();
					userlist = userlist.Substring (0, userlist.Length - 1);

					for (int i = 0; i < triggersonplace.Count; i++) {
						//plugin.Notify ("", userlist, triggersonplace [i].text);
						pushfeature.Notify ("", userlist, triggersonplace [i].text, "UserId");
					}
				}
				return true;

			} catch (Exception ex) {
				return false;
			}
		}
	}
}

