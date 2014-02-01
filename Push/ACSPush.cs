using System;
using RestSharp;
using System.Linq;

namespace GeoAPI
{
	public class ACSPush : IPush
	{
		private string APIToken { get; set; }

		private string BaseUrl { get; set; }

		private string SessionID { get; set; }

		private string UserName { get; set; }

		private string Password { get; set; }

		RestClient client = new RestClient ();

		public ACSPush (string BaseUrl, string APIToken, string UserName, string Password)
		{
			this.APIToken = APIToken;
			this.BaseUrl = BaseUrl;
			this.UserName = UserName;
			this.Password = Password;

		}

		/// <summary>
		/// Notify the specified channel, to_ids, payload and filterType.
		/// </summary>
		/// <param name="channel">Channel.</param>
		/// <param name="to_ids">To_ids.</param>
		/// <param name="payload">Payload.</param>
		/// <param name="filterType">Filter type. NOT USED IN ACS PUSH</param>
		public virtual string Notify (string channel, string to_ids, string payload, string filterType, string devicePlatform)
		{
			Login ();

			client.BaseUrl = BaseUrl;

			var request = new RestRequest ();
			request.Method = Method.POST;
			request.AddHeader ("Content-Type", "application/json");
			request.Resource = "push_notification/notify.json?key=" + this.APIToken;
			request.AddUrlSegment ("key", this.APIToken);
			request.AddCookie ("_session_id", SessionID);

			request.AddParameter ("channel", channel);
			request.AddParameter ("to_ids", to_ids);
			request.AddParameter ("payload", payload);
			IRestResponse response = client.Execute (request);

			return response.StatusCode.ToString ();

		}

		private void Login ()
		{

			client.BaseUrl = BaseUrl;

			var request = new RestRequest ();
			request.Method = Method.POST;
			request.AddHeader ("Content-Type", "application/json");
			client.CookieContainer = new System.Net.CookieContainer ();
			request.AddUrlSegment ("key", this.APIToken);
			request.Resource = "users/login.json?key=" + this.APIToken;

			request.AddParameter ("login", this.UserName);
			request.AddParameter ("password", this.Password);
			IRestResponse response = client.Execute (request);

			var acsSessionId = response.Cookies.SingleOrDefault (x => x.Name == "_session_id");
			this.SessionID = acsSessionId.Value.ToString ();

		}
	}
}

