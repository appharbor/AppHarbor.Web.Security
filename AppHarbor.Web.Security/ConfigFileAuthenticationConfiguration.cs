using System;
using System.Configuration;
using System.Web.Security;

namespace AppHarbor.Web.Security
{
	public sealed class ConfigFileAuthenticationConfiguration : ICookieAuthenticationConfiguration
	{
		public string CookieName
		{
			get
			{
				return FormsAuthentication.FormsCookieName;
			}
		}

		public bool SlidingExpiration
		{
			get
			{
				return FormsAuthentication.SlidingExpiration;
			}
		}

		public CookieProtection CookieProtection
		{
			get
			{
				return CookieProtection.All;
			}
		}

		public string LoginUrl
		{
			get
			{
				return FormsAuthentication.LoginUrl;
			}
		}

		public string EncryptionAlgorithm
		{
			get
			{
				return "rijndael";
			}
		}

		public string EncryptionKey
		{
			get
			{
				return ConfigurationManager.AppSettings["cookieauthentication.encryptionkey"];
			}
		}

		public string EncryptionIV
		{
			get
			{
				return ConfigurationManager.AppSettings["cookieauthentication.encryptioniv"];
			}
		}

		public string ValidationAlgorithm
		{
			get
			{
				return "hmacsha256";
			}
		}

		public string ValidationKey
		{
			get
			{
				return ConfigurationManager.AppSettings["cookieauthentication.validationkey"];
			}
		}

		public TimeSpan Timeout
		{
			get
			{
				return FormsAuthentication.Timeout;
			}
		}

		public bool RequireSSL
		{
			get
			{
				return FormsAuthentication.RequireSSL;
			}
		}
	}
}
