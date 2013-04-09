using System;
using System.IO;
using System.Security.Principal;

namespace AppHarbor.Web.Security
{
	public class AuthenticationCookie
	{
		private readonly short _cookieType;
		private readonly Guid _id;
		private readonly bool _persistent;
		private DateTime _issueDate;
		private readonly string _name;
		private readonly byte[] _tag;
		private readonly string[] _roles;

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
					_name = binaryReader.ReadString();
					var rolesLength = binaryReader.ReadInt16();
					_roles = new string[rolesLength];
					for (int i = 0; i < _roles.Length; i++)
					{
						_roles[i] = binaryReader.ReadString();
					}
					var tagLength = binaryReader.ReadInt16();
					if (tagLength == 0)
					{
						_tag = null;
					}
					else
					{
						_tag = binaryReader.ReadBytes(tagLength);
					}
				}
			}
		}

		public AuthenticationCookie(short cookieType, Guid id, bool persistent, string name, string[] roles = null, byte[] tag = null)
		{
			_cookieType = cookieType;
			_id = id;
			_persistent = persistent;
			_name = name;
			_roles = roles ?? new string[0];
			_tag = tag;
			_issueDate = DateTime.UtcNow;
		}

		public IPrincipal GetPrincipal()
		{
			var identity = new CookieIdentity(this);
			return new GenericPrincipal(identity, _roles);
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
					binaryWriter.Write(_name);
					if (_roles == null)
					{
						binaryWriter.Write((short)0);
					}
					else
					{
						binaryWriter.Write((short)_roles.Length);
						foreach (var role in _roles)
						{
							binaryWriter.Write(role);
						}
					}
					if (_tag == null)
					{
						binaryWriter.Write((short)0);
					}
					else
					{
						binaryWriter.Write((short)_tag.Length);
						binaryWriter.Write(_tag);
					}
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

		public string Name
		{
			get
			{
				return _name;
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

		public string[] Roles
		{
			get
			{
				return _roles;
			}
		}

		public byte[] Tag
		{
			get
			{
				return _tag;
			}
		}
	}
}
