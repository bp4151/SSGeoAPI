using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Admin;
using ServiceStack.ServiceInterface.Cors;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Extensions;
using ServiceStack.WebHost.Endpoints.Support;

namespace GeoAPI
{
	public class Global : System.Web.HttpApplication
	{
		public class AppHost : AppHostBase
		{
			//Tell Service Stack the name of your application and where to find your web services
			public AppHost () : base ("GeoAPI Web Services", typeof(PushService).Assembly)
			{

			}

			public override void Configure (Funq.Container container)
			{
				//register any dependencies your services use, e.g:
				//container.Register<ICacheClient>(new MemoryCacheClient());
				var appSettings = new AppSettings ();

				string baseUrl = appSettings.GetString ("BaseUrl") ?? "";
				string APIToken = appSettings.GetString ("APIToken") ?? "";
				string userName = appSettings.GetString ("UserName") ?? "";
				string password = appSettings.GetString ("Password") ?? "";

				//Plugins.Add (new SwaggerFeature ());
				Plugins.Add (new CorsFeature ("http://petstore.swagger.wordnik.com"));
				Plugins.Add (new CorsFeature ("*", "GET, POST, PUT, DELETE, OPTIONS", "content-type", false));
				//Plugins.Add (new RequestLogsFeature (){ RequiredRoles = new string[]{ } });

				//Plugin for push removed in favor of IoC/DI
				//Plugins.Add (new ACSPushFeature ());
				//Plugins.Add (new EverlivePushFeature ());

				//Use the string name when calling in the service to select which push feature to use
				//container.Register<IPush> ("EverlivePush", new EverlivePush (baseUrl, APIToken));
				container.Register<IPush> ("ACSPush", new ACSPush (baseUrl, APIToken, userName, password));

				SetConfig (new EndpointHostConfig { 
					DebugMode = true 
				});

				this.PreRequestFilters.Add ((req, resp) => {

				});

				this.RequestFilters.Add ((IHttpRequest httpReq, IHttpResponse httpResp, object requestDto) => {

					if (httpReq.Headers ["Authorization-API"] == null) {
						throw HttpError.Unauthorized ("No Authorization Header provided");
					}

					//ServiceStack instance API Key
					string storedAPIKey = appSettings.Get ("GeoAPIKey", "");
					//API Key passed from client
					string passedAPIKey = httpReq.Headers ["Authorization-API"];

					if (String.IsNullOrEmpty (storedAPIKey)) {
						throw HttpError.Unauthorized ("API Key not configured");
					} else if (storedAPIKey != passedAPIKey) {
						throw HttpError.Unauthorized ("API Key passed from the client was not found");
					}

				});

			}
		}
		//Initialize your application singleton
		protected void Application_Start (object sender, EventArgs e)
		{
			new AppHost ().Init ();

		}

		protected void Session_Start (Object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest (Object sender, EventArgs e)
		{
		}

		protected void Application_EndRequest (Object sender, EventArgs e)
		{
		}

		protected void Application_AuthenticateRequest (Object sender, EventArgs e)
		{
		}

		protected void Application_Error (Object sender, EventArgs e)
		{
		}

		protected void Session_End (Object sender, EventArgs e)
		{
		}

		protected void Application_End (Object sender, EventArgs e)
		{
		}
	}
}

