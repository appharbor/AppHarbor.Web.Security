using System;
using System.Text;
using System.Web.Security;

namespace AppHarbor.Web.Security
{
	public class CookieProtector : IDisposable 
	{
		private readonly Encryption _encryption;
		private readonly Validation _validation;

		public CookieProtector(ICookieAuthenticationConfiguration configuration)
		{
			_encryption = Encryption.Create(configuration.EncryptionAlgorithm, configuration.EncryptionKey);
			_validation = Validation.Create(configuration.ValidationAlgorithm, configuration.ValidationKey);
		}

		public bool Validate(string cookie, out string data)
		{
			byte[] cookieData;
			if (Validate(cookie, out cookieData))
			{
				data = Encoding.UTF8.GetString(cookieData);
				return true;
			}

			data = null;
			return false;
		}

		public bool Validate(string cookie, out byte[] data) 
		{
			data = null;
			try 
			{
				var versionedCookieData = Convert.FromBase64String(cookie);
				
				if (versionedCookieData.Length == 0 || versionedCookieData[0] != 0)
				{
					return false;
				}

				var cookieData = new byte[versionedCookieData.Length - 1];
				Buffer.BlockCopy(versionedCookieData, 1, cookieData, 0, cookieData.Length);

				if (!_validation.Validate(cookieData))
				{
					return false;
				}

				cookieData = _validation.StripSignature(cookieData);
				cookieData = _encryption.Decrypt(cookieData);

				data = cookieData;
				return true;
			}
			catch 
			{
				return false;
			}
		}

		public string Protect(string data)
		{
			return Protect(Encoding.UTF8.GetBytes(data));
		}

		public string Protect(byte[] data)
		{
			data = _encryption.Encrypt(data);
			data = _validation.Sign(data);

			var versionedData = new byte[data.Length + 1];
			Buffer.BlockCopy(data, 0, versionedData, 1, data.Length);
			return Convert.ToBase64String(versionedData);
		}

		public void Dispose()
		{
			_encryption.Dispose();
			_validation.Dispose();
		}
	}
}
