﻿using System;
using System.Web;

namespace AppHarbor.Web.Security
{
	using System.Security.Claims;

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
		
		public void SetCookie(ClaimsIdentity identity, bool persistent = false)
		{
			var cookie = new AuthenticationCookie(0, Guid.NewGuid(), persistent, identity);
			using (var protector = new CookieProtector(_configuration))
			{
				var httpCookie = new HttpCookie(_configuration.CookieName, protector.Protect(cookie.Serialize()))
				{
					HttpOnly = true,
					Secure = _configuration.RequireSSL,
				};
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
	}
}
