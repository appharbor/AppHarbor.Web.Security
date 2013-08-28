using System;
using System.IO;
using System.Security.Principal;

namespace AppHarbor.Web.Security
{
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Security.Claims;

	public class AuthenticationCookie
	{
		private readonly short _cookieType;
		private readonly Guid _id;
		private readonly bool _persistent;
		private DateTime _issueDate;
		private readonly ClaimsIdentity _identity;

		private AuthenticationCookie(byte[] data)
		{
			using (var memoryStream = new MemoryStream(data))
			{
				using (var binaryReader = new BinaryReader(memoryStream))
				{
					_cookieType = binaryReader.ReadInt16();
					_id = new Guid(binaryReader.ReadBytes(16));
					_persistent = binaryReader.ReadBoolean();
					_issueDate = DateTime.FromBinary(binaryReader.ReadInt64());
					var identityLength = binaryReader.ReadInt32();
					var identityBytes = binaryReader.ReadBytes(identityLength);
					var stream = new MemoryStream(identityBytes);
					var formatter = new BinaryFormatter();
					_identity = (ClaimsIdentity)formatter.Deserialize(stream);
				}
			}
		}

		public AuthenticationCookie(short cookieType, Guid id, bool persistent, ClaimsIdentity identity)
		{
			_cookieType = cookieType;
			_id = id;
			_persistent = persistent;
			_identity = identity;
			_issueDate = DateTime.UtcNow;
		}

		public IPrincipal GetPrincipal()
		{
			return new ClaimsPrincipal(_identity);
		}

		public byte[] Serialize()
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(_cookieType);
					binaryWriter.Write(_id.ToByteArray());
					binaryWriter.Write(_persistent);
					binaryWriter.Write(_issueDate.ToBinary());
					var formatter = new BinaryFormatter();
					var stream = new MemoryStream();
					formatter.Serialize(stream, _identity);
					binaryWriter.Write((int)stream.Length);
					binaryWriter.Write(stream.ToArray());
				}
				return memoryStream.ToArray();
			}
		}

		public static AuthenticationCookie Deserialize(byte[] data)
		{
			return new AuthenticationCookie(data);
		}

		public void Renew()
		{
			_issueDate = DateTime.UtcNow;
		}

		public bool IsExpired(TimeSpan validity)
		{
			return _issueDate.Add(validity) <= DateTime.UtcNow;
		}

		public DateTime IssueDate
		{
			get
			{
				return _issueDate;
			}
		}

		public bool Persistent
		{
			get
			{
				return _persistent;
			}
		}

		public short CookieType
		{
			get
			{
				return _cookieType;
			}
		}

		public Guid Id
		{
			get
			{
				return _id;
			}
		}

		public ClaimsIdentity Identity
		{
			get
			{
				return _identity;
			}
		}
	}
}
