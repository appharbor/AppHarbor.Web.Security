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

		private static string GetRequiredSetting(string name)
		{
			var setting = ConfigurationManager.AppSettings[name];
			if (setting != null)
			{
				return setting;
			}

			throw new Exception(string.Format("Required setting '{0}' not found.", name));
		}

		public byte[] EncryptionKey
		{
			get
			{
				return GetRequiredSetting("cookieauthentication.encryptionkey").GetByteArrayFromHexString();
			}
		}

		public string ValidationAlgorithm
		{
			get
			{
				return "hmacsha256";
			}
		}

		public byte[] ValidationKey
		{
			get
			{
				return GetRequiredSetting("cookieauthentication.validationkey").GetByteArrayFromHexString();
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
