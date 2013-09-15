using System;
using RestSharp;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Configuration;
using ServiceStack.Common.Web;
using System.Linq;
using System.Collections.Generic;

namespace GeoAPI
{
	public class EverlivePushFeature : IPlugin
	{
		/*
		 * "Filter": "{\"$or\":[{\"Id\":\"deviece id 1\"},{\"Id\":\"device id 2\"}]}"
		 */
		private string ELAPIToken { get; set; }

		private string ELBaseUrl { get; set; }

		private string ELSessionID { get; set; }

		private string ELUserName { get; set; }

		private string ELPassword { get; set; }

		public EverlivePushFeature ()
		{

		}

		public void Register (IAppHost appHost)
		{

			var appSettings = new AppSettings ();

			this.ELAPIToken = appSettings.GetString ("ELAPIToken");
			this.ELBaseUrl = appSettings.GetString ("ELBaseUrl");

		}

		/// <summary>
		/// 
		/// var notification = {
		///		"Filter": "{\"PlatformType\": 1}",
		///		"Message": "A generic message"
		///	};
		///
		/// "Filter": "{\"PlatformType\": 1}" // filter by platform
		/// "Filter": "{\"$or\":[{\"PushToken\":\"pushtoken\"},{\"PushToken\":\"pushtoken\"}]}" // targets specific devices
		///	"Filter": "{\"Parameters.MyIntValue\":2}" // filter by custom parameter 
		/// 
		/// $.ajax({
		///		type: "POST",
		///		url: 'https://api.everlive.com/v1/[apikey]/Push/Notifications',
		///		contentType: "application/json",
		///		data: JSON.stringify(notification),
		///		success: function(data){
		///			alert(JSON.stringify(data));
		///		},
		///		error: function(error){
		///			alert(JSON.stringify(error));
		///		}
		///	});
		/// 
		/// </summary>
		/// <param name="to_ids">To_ids.</param>
		/// <param name="payload">Payload.</param>
		public virtual string Notify (string channel, string to_ids, string payload)
		{
			IRestResponse response = null;
			try {

				char[] splitParam = { ',' };
				string[] toids = to_ids.Split (splitParam);

				string Filter = "";

				if (toids.Length > 0) {
					Filter = "{'$or':[";
					for (var i = 0; i < toids.Length; i++) {
						Filter = Filter + "{ 'UserId':'" + toids [i] + "'}";
					}
					Filter = Filter + "]}";
				}

				var client = new RestClient ();
				client.BaseUrl = ELBaseUrl;

				var request = new RestRequest ();
				request.Method = Method.POST;
				request.RequestFormat = RestSharp.DataFormat.Json;
				request.Resource = this.ELAPIToken + "/Push/Notifications";
				request.AddParameter ("Filter", Filter);
				request.AddParameter ("Message", payload);
				response = client.Execute (request);

				return response.StatusCode.ToString ();
			}
			catch(Exception ex)
			{
				response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
				response.StatusDescription = ex.Message;
				return response.StatusCode.ToString();

			}
		}
	}

	public class ACSPushFeature : IPlugin
	{
		private string ACSAPIToken { get; set; }

		private string ACSBaseUrl { get; set; }

		private string ACSSessionID { get; set; }

		private string ACSUserName { get; set; }

		private string ACSPassword { get; set; }

		public ACSPushFeature ()
		{

		}

		public void Register (IAppHost appHost)
		{

			var appSettings = new AppSettings ();

			this.ACSAPIToken = appSettings.GetString ("ACSAPIToken");
			this.ACSBaseUrl = appSettings.GetString ("ACSBaseUrl");
			this.ACSUserName = appSettings.GetString ("ACSUserName");
			this.ACSPassword = appSettings.GetString ("ACSPassword");

			Login ();
		}

		public virtual string Notify (string channel, string to_ids, string payload)
		{
			var client = new RestClient ();
			client.BaseUrl = ACSBaseUrl;

			var request = new RestRequest ();
			request.Method = Method.POST;
			request.Resource = "push_notification/notify.json?key=" + this.ACSAPIToken;
			request.AddCookie ("_session_id", ACSSessionID);
			//request.AddHeader ("_session_id", ACSSessionID);
			request.AddParameter ("channel", channel);
			request.AddParameter ("to_ids", to_ids);
			request.AddParameter ("payload", payload);
			IRestResponse response = client.Execute (request);

			return response.StatusCode.ToString ();

		}

		private void Login ()
		{
			var client = new RestClient ();
			client.BaseUrl = ACSBaseUrl;

			var request = new RestRequest ();
			request.Method = Method.POST;
			request.Resource = "users/login.json?key=" + this.ACSAPIToken;
			request.AddParameter ("login", this.ACSUserName);
			request.AddParameter ("password", this.ACSPassword);
			IRestResponse response = client.Execute (request);

			var acsSessionId = response.Cookies.SingleOrDefault (x => x.Name == "_session_id");
			this.ACSSessionID = acsSessionId.Value.ToString ();

		}
	}
}

