using System;
using System.Security.Cryptography;

namespace AppHarbor.Web.Security 
{
	public abstract class Validation : IDisposable
	{
		public virtual void Dispose()
		{
		}

		public abstract byte[] ComputeSignature(byte[] data);
		public abstract byte[] Sign(byte[] data);
		public abstract byte[] StripSignature(byte[] signedMessage);
		public abstract bool Validate(byte[] signedMessage);

		public static Validation Create(string key)
		{
			return Create(key.GetByteArrayFromHexString());
		}

		public static Validation Create(byte[] key)
		{
			return Create(KeyedHashAlgorithm.Create(), key);
		}

		public static Validation Create(string algorithm, byte[] key)
		{
			return Create(KeyedHashAlgorithm.Create(algorithm), key);
		}

		public static Validation Create(KeyedHashAlgorithm algorithm, byte[] key)
		{
			return new KeyedHashValidation(algorithm, key);
		}

		public static Validation Create(string algorithm, string key)
		{
			return Create(KeyedHashAlgorithm.Create(algorithm), key.GetByteArrayFromHexString());
		}
		
		public static Validation Create<T>(byte[] key) where T : KeyedHashAlgorithm, new()
		{
			return new KeyedHashValidation<T>(key);
		}
		
		public static Validation Create<T>(string key) where T : KeyedHashAlgorithm, new()
		{
			return Create<T>(key.GetByteArrayFromHexString());
		}
	}
}
