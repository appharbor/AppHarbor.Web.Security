using System;
using System.Web;

namespace AppHarbor.Web.Security
{
	public sealed class CookieAuthenticator : IAuthenticator
	{
		private readonly ICookieAuthenticationConfiguration _configuration;
		
		public CookieAuthenticator()
			: this(new ConfigFileAuthenticationConfiguration())
		{
		}

		public CookieAuthenticator(ICookieAuthenticationConfiguration configuration)
		{
			_configuration = configuration;
		}
		
		public void SetCookie(string username, bool persistent = false, byte[] tag = null)
		{
			var cookie = new AuthenticationCookie(0, Guid.NewGuid(), persistent, username, tag);
			using (var protector = new CookieProtector(_configuration))
			{
				var httpCookie = new HttpCookie(_configuration.CookieName, protector.Protect(cookie.Serialize()))
				{
					HttpOnly = true,
					Secure = _configuration.RequireSSL,
				};
				if (!persistent)
				{
					httpCookie.Expires = cookie.IssueDate + _configuration.Timeout;
				}

				HttpContext.Current.Response.Cookies.Add(httpCookie);
			}
		}

		public void SignOut()
		{
			HttpContext.Current.Response.Cookies.Remove(_configuration.CookieName);
			HttpContext.Current.Response.Cookies.Add(new HttpCookie(_configuration.CookieName, "") { Expires = DateTime.UtcNow.AddMonths(-100) });
		}
	}
}
