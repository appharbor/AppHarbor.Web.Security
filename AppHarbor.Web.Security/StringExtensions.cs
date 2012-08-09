using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AppHarbor.Web.Security
{
	internal static class StringExtensions
	{
		public static byte[] GetByteArrayFromHexString(this string s)
		{
			return SoapHexBinary.Parse(s).Value;
		}

		public static string GetHexString(this byte[] b)
		{
			return new SoapHexBinary(b).ToString();
		}
	}
}
