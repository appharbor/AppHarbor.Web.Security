using System;
using System.Web;

namespace AppHarbor.Web.Security
{
	public sealed class CookieAuthenticator : IAuthenticator
	{
		private readonly ICookieAuthenticationConfiguration _configuration;
		private readonly HttpContextBase _context;
		
		public CookieAuthenticator()
			: this(new ConfigFileAuthenticationConfiguration(), new HttpContextWrapper(HttpContext.Current))
		{
		}

		public CookieAuthenticator(ICookieAuthenticationConfiguration configuration, HttpContextBase context)
		{
			_configuration = configuration;
			_context = context;
		}
		
		public void SetCookie(string username, bool persistent = false, string[] roles = null, byte[] tag = null)
		{
			var cookie = new AuthenticationCookie(0, Guid.NewGuid(), persistent, username, roles, tag);
			using (var protector = new CookieProtector(_configuration))
			{
				var httpCookie = new HttpCookie(_configuration.CookieName, protector.Protect(cookie.Serialize()))
				{
					HttpOnly = true,
					Secure = _configuration.RequireSSL,
				};

                if (!string.IsNullOrEmpty(_configuration.Domain))
                {
                    httpCookie.Domain = _configuration.Domain;
                }

				if (persistent)
				{
					httpCookie.Expires = cookie.IssueDate + _configuration.Timeout;
				}

				_context.Response.Cookies.Add(httpCookie);
			}
		}

		public void SignOut()
		{
			_context.Response.Cookies.Remove(_configuration.CookieName);
			_context.Response.Cookies.Add(new HttpCookie(_configuration.CookieName, "")
			{
				Expires = DateTime.UtcNow.AddMonths(-100),
			});
		}

	    public byte[] GetTagData()
	    {
            var cookie = _context.Request.Cookies[_configuration.CookieName];
            if (cookie != null)
            {
                using(var protector = new CookieProtector(_configuration))
                {
                    byte[] data;
                    protector.Validate(cookie.Value, out data);
                    var authenticationCookie = AuthenticationCookie.Deserialize(data);
                    return authenticationCookie.Tag;
                }
            }

	        return null;
	    }
	}
}
