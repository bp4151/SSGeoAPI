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
using ServiceStack.ServiceInterface.Cors;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Extensions;
using ServiceStack.WebHost.Endpoints.Support;
using ServiceStack.Api.Swagger;

namespace GeoAPI
{
	public class Global : System.Web.HttpApplication
	{
		

		public class AppHost : AppHostBase
		{
			//Tell Service Stack the name of your application and where to find your web services
			public AppHost () : base("GeoAPI Web Services", typeof(TestService).Assembly)
			{

			}

			public override void Configure (Funq.Container container)
			{
				//register any dependencies your services use, e.g:
				//container.Register<ICacheClient>(new MemoryCacheClient());

				Plugins.Add (new SwaggerFeature ());

				Routes.Add<LocationRequest> ("/Location/Update/", "POST");

				SetConfig (new EndpointHostConfig { 
					DebugMode = true 
				});

				this.PreRequestFilters.Add ((req, resp) => {

				});

				this.RequestFilters.Add ((IHttpRequest httpReq, IHttpResponse httpResp, object requestDto) => {
					var appSettings = new AppSettings ();

					if (httpReq.Headers ["Authorization-API"] == null) {
						throw HttpError.Unauthorized ("No Authorization Header provided");
					}

					string storedAPIKey = appSettings.Get ("GeoAPIKey", "");
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

