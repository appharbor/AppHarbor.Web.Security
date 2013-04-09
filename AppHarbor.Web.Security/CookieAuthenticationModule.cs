using System;
using System.Linq;
using System.Web;

namespace AppHarbor.Web.Security
{
	public sealed class CookieAuthenticationModule : IHttpModule
	{
		private readonly ICookieAuthenticationConfiguration _configuration;

		public CookieAuthenticationModule()
			: this(new ConfigFileAuthenticationConfiguration())
		{
		}

		public CookieAuthenticationModule(ICookieAuthenticationConfiguration configuration)
		{
			_configuration = configuration;
		}

		private void OnAuthenticateRequest(object sender, EventArgs e)
		{
			var context = ((HttpApplication)sender).Context;
			var cookie = context.Request.Cookies[_configuration.CookieName];
			if (cookie != null)
			{
				var protector = new CookieProtector(_configuration);
				try
				{
					byte[] data;
					var cookieData = protector.Validate(cookie.Value, out data);
					var authenticationCookie = AuthenticationCookie.Deserialize(data);
					if (!authenticationCookie.IsExpired(_configuration.Timeout))
					{
						context.User = authenticationCookie.GetPrincipal();
						RenewCookieIfExpiring(context, protector, authenticationCookie);
					}
				}
				catch
				{
					// do not leak any information if an exception was thrown.
					// simply don't set the context.User property.
				}
				finally
				{
					if (protector != null)
					{
						protector.Dispose();
					}
				}
			}

			if (IsLoginPage(context.Request))
			{
				context.SkipAuthorization = true;
			}
		}

		private void RenewCookieIfExpiring(HttpContext context, CookieProtector protector, AuthenticationCookie authenticationCookie)
		{
			if (!_configuration.SlidingExpiration || !authenticationCookie.IsExpired(TimeSpan.FromTicks(_configuration.Timeout.Ticks / 2)))
			{
				return;
			}
			authenticationCookie.Renew();
			context.Response.Cookies.Remove(_configuration.CookieName);
			var newCookie = new HttpCookie(_configuration.CookieName, protector.Protect(authenticationCookie.Serialize()))
			{
				HttpOnly = true,
				Secure = _configuration.RequireSSL,
			};
			if (authenticationCookie.Persistent)
			{
				newCookie.Expires = authenticationCookie.IssueDate + _configuration.Timeout;
			}
			context.Response.Cookies.Add(newCookie);
		}

		private bool IsLoginPage(HttpRequest request)
		{
			try
			{
				var loginUrl = _configuration.LoginUrl;
				if (!loginUrl.StartsWith("/", StringComparison.Ordinal) && !loginUrl.StartsWith("~/"))
				{
					loginUrl = "~/" + loginUrl;
				}

				return string.Equals(request.PhysicalPath, request.MapPath(loginUrl), StringComparison.OrdinalIgnoreCase);
			}
			catch
			{
				return false; // Cannot risk it. If any exception is thrown, err on the safe side: assume it's not the login page.
			}
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			var context = ((HttpApplication)sender).Context;
			var response = context.Response;
			var request = context.Request;
			if (response.Cookies.Keys.Cast<string>().Contains(_configuration.CookieName))
			{
				response.Cache.SetCacheability(HttpCacheability.NoCache, "Set-Cookie");
			}
			if (response.StatusCode == 401 && !request.QueryString.AllKeys.Contains("ReturnUrl"))
			{
				var delimiter = "?";
				var loginUrl = _configuration.LoginUrl;
				if (loginUrl.Contains("?"))
				{
					delimiter = "&";
				}
				response.Redirect(loginUrl + delimiter + "ReturnUrl=" + HttpUtility.UrlEncode(context.Request.RawUrl), false);
			}
		}

		public void Init(HttpApplication context)
		{
			context.AuthenticateRequest += OnAuthenticateRequest;
			context.EndRequest += OnEndRequest;
		}

		public void Dispose()
		{
		}
	}
}
