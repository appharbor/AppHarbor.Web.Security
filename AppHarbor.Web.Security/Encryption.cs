using System;
using System.Security.Cryptography;

namespace AppHarbor.Web.Security
{
	public abstract class Encryption : IDisposable
	{
		public virtual void Dispose()
		{
		}

		public abstract byte[] Encrypt(byte[] valueBytes, byte[] initializationVector = null);
		public abstract byte[] Decrypt(byte[] encryptedValue, byte[] initializationVector = null);

		public static Encryption Create(SymmetricAlgorithm algorithm, byte[] secretKey)
		{
			return new SymmetricEncryption(algorithm, secretKey);
		}

		public static Encryption Create(string algorithm, byte[] secretKey)
		{
			return Create(SymmetricAlgorithm.Create(algorithm), secretKey);
		}

		public static Encryption Create(string secretKey)
		{
			return Create(SymmetricAlgorithm.Create(), secretKey.GetByteArrayFromHexString());
		}

		public static Encryption Create(string algorithm, string secretKey)
		{
			return Create(SymmetricAlgorithm.Create(algorithm), secretKey.GetByteArrayFromHexString());
		}

		public static Encryption Create<T>(byte[] secretKey) where T : SymmetricAlgorithm, new()
		{
			return new SymmetricEncryption<T>(secretKey);
		}

		public static Encryption Create<T>(string secretKey) where T: SymmetricAlgorithm, new()
		{
			return Create<T>(secretKey.GetByteArrayFromHexString());
		}
	}
}
