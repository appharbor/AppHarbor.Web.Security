using System;
using System.Security.Cryptography;

namespace KeyGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var rijndael = new RijndaelManaged())
			{
				rijndael.GenerateKey();
				rijndael.GenerateIV();
				using (var hmacsha1 = new HMACSHA256())
				{
					hmacsha1.Initialize();
					Console.WriteLine(template,
						Convert.ToBase64String(rijndael.Key), 
						Convert.ToBase64String(rijndael.IV),
						Convert.ToBase64String(hmacsha1.Key));
				}
			}
			Console.ReadKey();
		}

		const string template = @"<add key=""cookieauthentication.encryptionkey"" value=""{0}""/>
<add key=""cookieauthentication.encryptioniv"" value=""{1}""/>
<add key=""cookieauthentication.validationkey"" value=""{2}""/>";
	}
}
