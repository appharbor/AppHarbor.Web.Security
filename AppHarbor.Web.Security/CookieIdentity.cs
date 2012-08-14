using System;
using System.Security.Principal;

namespace AppHarbor.Web.Security
{
	// Ideally, we'd inherit from GenericIdentity and would not have MarshalByRefObject here.
	// However, Cassini has a long time bug that makes it throw a SerializationException
	// at runtime.  Inheriting from MarshalByRefObject works around that bug.
	[Serializable]
	public class CookieIdentity : MarshalByRefObject, IIdentity
	{
		private readonly AuthenticationCookie _cookie;

		public CookieIdentity(AuthenticationCookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}

			_cookie = cookie;
		}
		
		public bool IsAuthenticated
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Name);
			}
		}

		public string AuthenticationType
		{
			get
			{
				return "cookie";
			}
		}

		public string Name
		{
			get
			{
				return _cookie.Name;
			}
		}
	}
}
