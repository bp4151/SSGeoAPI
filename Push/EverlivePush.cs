using System;
using RestSharp;
using Telerik.Everlive.Sdk.Core;
using Telerik.Everlive.Sdk.Core.Model.System.Push;

namespace GeoAPI
{
	public class EverlivePush : IPush
	{
		private string APIToken { get; set; }

		private string BaseUrl { get; set; }

		public EverlivePush (string BaseURL, string APIToken)
		{
			this.APIToken = APIToken;
			this.BaseUrl = BaseUrl;
		}

		/// <summary>
		/// Notify the specified channel, to_ids, payload and filterType.
		/// </summary>
		/// <param name="channel">Channel. NOT USED IN EVERLIVE</param>
		/// <param name="to_ids">To_ids.</param>
		/// <param name="payload">Payload.</param>
		/// <param name="filterType">Filter type.</param>
		public virtual string Notify (string channel, string to_ids, string payload, string filterType)
		{
			IRestResponse response = null;
			try {

				char[] splitParam = { ',' };
				string[] toids = to_ids.Split (splitParam);

				string Filter = "";

				if (toids.Length > 0) {
					Filter = "{\"$or\":[";
					for (var i = 0; i < toids.Length; i++) {
						Filter = Filter + "{ \"" + filterType + "\":\"" + toids [i] + "\"},";
					}
					Filter = Filter.Substring(0, Filter.Length - 1);
					Filter = Filter + "]}";
				}

				//string notification = "{ 'Filter': " + Filter + ", 'Message': '" + payload + "'}";

				EverliveApp elApp = new EverliveApp(this.APIToken);
				var notification = new PushNotification(payload);
				notification.Filter = Filter;
				elApp.WorkWith().Push().Notifications().Create(notification).ExecuteSync();
				/*
				var client = new RestClient ();
				client.BaseUrl = ELBaseUrl;

				var request = new RestRequest ();
				request.Method = Method.POST;
				request.RequestFormat = RestSharp.DataFormat.Json;
				request.Resource = this.ELAPIToken + "/Push/Notifications";
				//request.AddParameter ("data", notification);
				request.AddBody(notification);
				response = client.Execute (request);
				*/
				return "200";
			}
			catch(Exception ex)
			{
				response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
				response.StatusDescription = ex.Message;
				return response.StatusCode.ToString();

			}
		}
	}
}

