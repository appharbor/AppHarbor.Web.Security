using System.IO;
using System.Security.Cryptography;

namespace AppHarbor.Web.Security
{
	public class SymmetricEncryption : Encryption
	{
		private readonly SymmetricAlgorithm _algorithm;
		private readonly byte[] _secretKey;
		private readonly byte[] _initializationVector;

		public SymmetricEncryption(SymmetricAlgorithm algorithm, byte[] secretKey, byte[] initializationVector)
		{
			_algorithm = algorithm;
			_secretKey = secretKey;
			_initializationVector = initializationVector;
		}

		public override void Dispose()
		{
			_algorithm.Dispose();
		}

		public override byte[] Encrypt(byte[] valueBytes)
		{
			using (var output = new MemoryStream())
			{
				using (var cryptoOutput = new CryptoStream(output, _algorithm.CreateEncryptor(_secretKey, _initializationVector), CryptoStreamMode.Write))
				{
					cryptoOutput.Write(valueBytes, 0, valueBytes.Length);
					cryptoOutput.Close();
				}

				return output.ToArray();
			}
		}

		public override byte[] Decrypt(byte[] encryptedValue)
		{
			using (var output = new MemoryStream())
			{
				using (var cryptoOutput = new CryptoStream(output, _algorithm.CreateDecryptor(_secretKey, _initializationVector), CryptoStreamMode.Write))
				{
					cryptoOutput.Write(encryptedValue, 0, encryptedValue.Length);
					cryptoOutput.Close();
				}

				return output.ToArray();
			}
		}
	}

	public sealed class SymmetricEncryption<T> : SymmetricEncryption where T : SymmetricAlgorithm, new()
	{
		public SymmetricEncryption(byte[] secretKey, byte[] initializationVector)
			: base(new T(), secretKey, initializationVector)
		{
		}
	}
}
