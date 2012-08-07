using System;
using System.Security.Principal;

namespace AppHarbor.Web.Security
{
	// Ideally, we'd inherit from GenericIdentity and would not have MarshalByRefObject here.
	// However, Cassini has a long time bug that makes it throw a SerializationException
	// at runtime.  Inheriting from MarshalByRefObject works around that bug.
	[Serializable]
	public class CookieIdentity : MarshalByRefObject, IIdentity
	{
		public CookieIdentity(string name, byte[] tag = null) 
		{
			Name = name;
			Tag = tag;
		}
		
		public bool IsAuthenticated
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Name);
			}
		}

		public string AuthenticationType
		{
			get
			{
				return "cookie";
			}
		}

		public string Name
		{
			get;
			private set;
		}

		public byte[] Tag
		{
			get;
			private set;
		}
	}
}
