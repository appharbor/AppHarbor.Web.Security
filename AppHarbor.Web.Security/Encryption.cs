using System;
using System.Security.Cryptography;

namespace AppHarbor.Web.Security
{
	public abstract class Encryption : IDisposable
	{
		public virtual void Dispose()
		{
		}

		public abstract byte[] Encrypt(byte[] valueBytes);
		public abstract byte[] Decrypt(byte[] encryptedValue);

		public static Encryption Create(SymmetricAlgorithm algorithm, byte[] secretKey, byte[] initializationVector)
		{
			return new SymmetricEncryption(algorithm, secretKey, initializationVector);
		}

		public static Encryption Create(string secretKey, string initializationVector)
		{
			return Create(SymmetricAlgorithm.Create(), Convert.FromBase64String(secretKey), Convert.FromBase64String(initializationVector));
		}

		public static Encryption Create(string algorithm, string secretKey, string initializationVector)
		{
			return Create(SymmetricAlgorithm.Create(algorithm), Convert.FromBase64String(secretKey), Convert.FromBase64String(initializationVector));
		}

		public static Encryption Create<T>(byte[] secretKey, byte[] initializationVector) where T : SymmetricAlgorithm, new()
		{
			return new SymmetricEncryption<T>(secretKey, initializationVector);
		}

		public static Encryption Create<T>(string secretKey, string initializationVector) where T: SymmetricAlgorithm, new()
		{
			return Create<T>(Convert.FromBase64String(secretKey), Convert.FromBase64String(initializationVector));
		}
	}
}
