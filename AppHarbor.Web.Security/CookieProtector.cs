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
			switch (configuration.CookieProtection)
			{
				case CookieProtection.None:
					break;
				case CookieProtection.Encryption:
					_encryption = Encryption.Create(configuration.EncryptionAlgorithm, configuration.EncryptionKey, configuration.EncryptionIV);
					break;
				case CookieProtection.Validation:
					_validation = Validation.Create(configuration.ValidationAlgorithm, configuration.ValidationKey);
					break;
				default:
					_encryption = Encryption.Create(configuration.EncryptionAlgorithm, configuration.EncryptionKey, configuration.EncryptionIV);
					_validation = Validation.Create(configuration.ValidationAlgorithm, configuration.ValidationKey);
					break;
			}
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
				var cookieData = Convert.FromBase64String(cookie);

				if (_validation != null)
				{
					if (_validation.Validate(cookieData))
					{
						cookieData = _validation.StripSignature(cookieData);
					}
					else
					{
						return false;
					}
				}

				if (_encryption != null)
				{
					cookieData = _encryption.Decrypt(cookieData);
				}

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
			if (_encryption != null)
			{
				data = _encryption.Encrypt(data);
			}

			if (_validation != null)
			{
				data = _validation.Sign(data);
			}

			return Convert.ToBase64String(data);
		}

		public void Dispose()
		{
			if (_encryption != null)
			{
				_encryption.Dispose();
			}

			if (_validation != null)
			{
				_validation.Dispose();
			}
		}
	}
}
