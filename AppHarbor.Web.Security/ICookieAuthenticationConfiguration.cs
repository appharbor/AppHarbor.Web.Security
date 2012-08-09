using System;
using System.Web.Security;

namespace AppHarbor.Web.Security
{
	public interface ICookieAuthenticationConfiguration
	{
		string CookieName { get; }
		bool SlidingExpiration { get; }
		TimeSpan Timeout { get; }
		CookieProtection CookieProtection { get; }
		string LoginUrl { get; }
		string EncryptionAlgorithm { get; }
		byte[] EncryptionKey { get; }
		byte[] EncryptionIV { get; }
		string ValidationAlgorithm { get; }
		byte[] ValidationKey { get; }
		bool RequireSSL { get; }
	}
}
