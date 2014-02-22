using System;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Configuration;
using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;
using System.Text;

namespace GeoAPI.Utility
{
	public static class Push
	{
		public static bool Run (IAppHost appHost, AppSettings appSettings, ObjectId place_id, List<string> usersInPlace, string message, string devicePlatform)
		{
			string pushPlatform = appSettings.Get ("PushPlatform", "");
			string channel = appSettings.Get ("Channel", "");
			string pushIDType = appSettings.Get ("PushIDType", "");

			try {

				var pushfeature = appHost.GetContainer ().ResolveNamed<IPush> (pushPlatform);			

				StringBuilder sb = new StringBuilder ();
				for (int i = 0; i < usersInPlace.Count; i++) {
					sb.Append (usersInPlace [i] + ",");
				}
				string userlist = sb.ToString ();
				if (userlist.Length > 0) {
					userlist = userlist.Substring (0, userlist.Length - 1);
					pushfeature.Notify (channel, userlist, message, pushIDType, devicePlatform);
				}
			
				return true;

			} catch (Exception ex) {
				throw ex;
			}
		}
	}
}

