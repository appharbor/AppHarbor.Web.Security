using System.Web;
using System.Web.Security;

namespace AppHarbor.Web.Security
{
	public static class CookieAuthentication
	{
		public static void RedirectFromLoginPage(string userName, bool createPersistentCookie = false, byte[] tag = null) 
		{
			SetAuthCookie(userName, createPersistentCookie, tag);
			var context = HttpContext.Current;
			var returnUrl = context.Request.QueryString["ReturnUrl"];
			if (!string.IsNullOrWhiteSpace(returnUrl))
			{
				returnUrl = FormsAuthentication.DefaultUrl;
			}
			context.Response.Redirect(returnUrl);
		}

		public static void SetAuthCookie(string userName, bool createPersistentCookie = false, byte[] tag = null)
		{
			var cookieAuthenticator = new CookieAuthenticator();
			cookieAuthenticator.SetCookie(userName, createPersistentCookie, tag);
		}

		public static void SignOut()
		{
			var cookieAuthenticator = new CookieAuthenticator();
			cookieAuthenticator.SignOut();
		}
	}
}
