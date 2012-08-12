using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AppHarbor.Web.Security;
using AuthenticationExample.Web.Mvc;
using AuthenticationExample.Web.PersistenceSupport;
using StructureMap;

namespace AuthenticationExample.Web
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory());

			ObjectFactory.Initialize(x =>
			{
				x.For<IRepository>()
					.Use(new InMemoryRepository());
				x.For<HttpContextBase>()
					.Use(() => new HttpContextWrapper(HttpContext.Current));
				x.For<ICookieAuthenticationConfiguration>()
					.Use<ConfigFileAuthenticationConfiguration>();
				x.For<IAuthenticator>()
					.Use<CookieAuthenticator>();
			});

			AreaRegistration.RegisterAllAreas();

			RegisterRoutes(RouteTable.Routes);
		}
	}
}
