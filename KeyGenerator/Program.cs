using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;

class Program
{
	static void Main(string[] args)
	{
		using (var rijndael = new RijndaelManaged())
		using (var hmacsha256 = new HMACSHA256())
		{
			rijndael.GenerateKey();
			hmacsha256.Initialize();
			Console.WriteLine(template, new SoapHexBinary(rijndael.Key), new SoapHexBinary(hmacsha256.Key));
		}

		Console.WriteLine("press any key to exit...");
		Console.ReadKey();
	}

	const string template = @"<add key=""cookieauthentication.encryptionkey"" value=""{0}""/>
<add key=""cookieauthentication.validationkey"" value=""{1}""/>";
}
