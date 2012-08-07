using System;
using System.Linq;
using System.Security.Cryptography;

namespace AppHarbor.Web.Security
{
	public class KeyedHashValidation : Validation
	{
		private readonly KeyedHashAlgorithm _algorithm;

		public KeyedHashValidation(KeyedHashAlgorithm algorithm, byte[] secretKey)
		{
			_algorithm = algorithm;
			_algorithm.Key = secretKey;
		}

		public override void Dispose()
		{
			_algorithm.Dispose();
		}

		public override byte[] ComputeSignature(byte[] data)
		{
			return ComputeSignature(data, 0, data.Length);
		}

		private byte[] ComputeSignature(byte[] data, int offset, int count)
		{
			return _algorithm.ComputeHash(data, offset, count);
		}

		public override byte[] Sign(byte[] data)
		{
			byte[] signedMessage = new byte[data.Length + _algorithm.HashSize / 8];
			Buffer.BlockCopy(data, 0, signedMessage, 0, data.Length);
			Buffer.BlockCopy(ComputeSignature(data), 0, signedMessage, data.Length, _algorithm.HashSize / 8);
			return signedMessage;
		}

		public override byte[] StripSignature(byte[] signedMessage)
		{
			var data = new byte[signedMessage.Length - _algorithm.HashSize / 8];
			Buffer.BlockCopy(signedMessage, 0, data, 0, data.Length);
			return data;
		}

		public override bool Validate(byte[] signedMessage)
		{
			return Validate(signedMessage, signedMessage.Length - _algorithm.HashSize / 8);
		}

		private bool Validate(byte[] signedMessage, int dataLength)
		{
			byte[] validSignature = ComputeSignature(signedMessage, 0, dataLength);
			return validSignature.SequenceEqual(signedMessage.Skip(dataLength));
		}
	}

	public class KeyedHashValidation<T> : KeyedHashValidation where T : KeyedHashAlgorithm, new()
	{
		public KeyedHashValidation(byte[] key)
			: base(new T(), key)
		{
		}
	}
}
