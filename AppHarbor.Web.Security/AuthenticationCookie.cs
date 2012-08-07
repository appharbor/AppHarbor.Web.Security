using System;
using System.IO;

namespace AppHarbor.Web.Security
{
	public class AuthenticationCookie
	{
		private readonly int _version;
		private readonly Guid _id;
		private readonly bool _persistent;
		private DateTime _issueDate;
		private readonly string _name;
		private readonly byte[] _tag;

		private AuthenticationCookie(byte[] data)
		{
			using (var memoryStream = new MemoryStream(data))
			{
				using (var binaryReader = new BinaryReader(memoryStream))
				{
					_version = binaryReader.ReadInt32();
					_id = new Guid(binaryReader.ReadBytes(16));
					_persistent = binaryReader.ReadBoolean();
					_issueDate = DateTime.FromBinary(binaryReader.ReadInt64());
					_name = binaryReader.ReadString();
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

		public AuthenticationCookie(int version, Guid id, bool persistent, string name, byte[] tag = null)
		{
			_version = version;
			_id = id;
			_persistent = persistent;
			_name = name;
			_tag = tag;
			_issueDate = DateTime.UtcNow;
		}

		public byte[] Serialize()
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(_version);
					binaryWriter.Write(_id.ToByteArray());
					binaryWriter.Write(_persistent);
					binaryWriter.Write(_issueDate.ToBinary());
					binaryWriter.Write(_name);
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
			return !Persistent && _issueDate.Add(validity) <= DateTime.UtcNow;
		}

		public int Version
		{
			get
			{
				return _version;
			}
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

		public Guid Id
		{
			get
			{
				return _id;
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
