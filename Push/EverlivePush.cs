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
		public virtual string Notify (string channel, string to_ids, string message, string filterType, string devicePlatform)
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
				//TODO: finish changes to push notification based on device type
				EverliveApp elApp = new EverliveApp(this.APIToken);

				var notification = new PushNotification();
				notification.Message = message;
				/*
				if (devicePlatform.ToUpper() == "IOS")
				{
					notification.iOS = new Telerik.Everlive.Sdk.Core.Model.System.Push.IOS.IOSNotification();
					notification.iOS.CustomProperties.Add("aps.alert", message);
				}
				else if (devicePlatform.ToUpper() == "ANDROID")
				{
					notification.Message = message;
				}
				*/
				notification.Filter = Filter;
				elApp.WorkWith().Push().Notifications().Create(notification).ExecuteSync();

				return "200";
			}
			catch(Exception ex)
			{
				throw ex;

			}
		}
	}
}

