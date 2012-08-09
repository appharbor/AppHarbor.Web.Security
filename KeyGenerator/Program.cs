using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;

class Program
{
	static void Main(string[] args)
	{
		using (var rijndael = new RijndaelManaged())
		{
			rijndael.GenerateKey();
			rijndael.GenerateIV();
			using (var hmacsha256 = new HMACSHA256())
			{
				hmacsha256.Initialize();
				Console.WriteLine(template,
					new SoapHexBinary(rijndael.Key), 
					new SoapHexBinary(rijndael.IV),
					new SoapHexBinary(hmacsha256.Key));
			}
		}
		Console.ReadKey();
	}

	const string template = @"<add key=""cookieauthentication.encryptionkey"" value=""{0}""/>
<add key=""cookieauthentication.encryptioniv"" value=""{1}""/>
<add key=""cookieauthentication.validationkey"" value=""{2}""/>";
}
