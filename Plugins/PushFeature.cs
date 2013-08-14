using System;
using RestSharp;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Configuration;
using ServiceStack.Common.Web;
using System.Linq;

namespace GeoAPI
{
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

