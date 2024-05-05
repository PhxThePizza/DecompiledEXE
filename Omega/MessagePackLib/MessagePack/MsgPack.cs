using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MessagePackLib.MessagePack
{
	// Token: 0x0200001E RID: 30
	public class MsgPack : IEnumerable
	{
		// Token: 0x06000089 RID: 137 RVA: 0x000024DB File Offset: 0x000006DB
		private void SetName(string value)
		{
			this.name = value;
			this.lowerName = this.name.ToLower();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004A08 File Offset: 0x00002C08
		private void Clear()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Clear();
			}
			this.children.Clear();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004A4C File Offset: 0x00002C4C
		private MsgPack InnerAdd()
		{
			MsgPack msgPack = new MsgPack();
			msgPack.parent = this;
			this.children.Add(msgPack);
			return msgPack;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004A74 File Offset: 0x00002C74
		private int IndexOf(string name)
		{
			int num = -1;
			int result = -1;
			string text = name.ToLower();
			foreach (MsgPack msgPack in this.children)
			{
				num++;
				if (text.Equals(msgPack.lowerName))
				{
					result = num;
					break;
				}
			}
			return result;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004AF0 File Offset: 0x00002CF0
		public MsgPack FindObject(string name)
		{
			int num = this.IndexOf(name);
			if (num == -1)
			{
				return null;
			}
			return this.children[num];
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000024F5 File Offset: 0x000006F5
		private MsgPack InnerAddMapChild()
		{
			if (this.valueType != MsgPackType.Map)
			{
				this.Clear();
				this.valueType = MsgPackType.Map;
			}
			return this.InnerAdd();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00002516 File Offset: 0x00000716
		private MsgPack InnerAddArrayChild()
		{
			if (this.valueType != MsgPackType.Array)
			{
				this.Clear();
				this.valueType = MsgPackType.Array;
			}
			return this.InnerAdd();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00002537 File Offset: 0x00000737
		public MsgPack AddArrayChild()
		{
			return this.InnerAddArrayChild();
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004B1C File Offset: 0x00002D1C
		private void WriteMap(Stream ms)
		{
			int count = this.children.Count;
			if (count <= 15)
			{
				byte value = 128 + (byte)count;
				ms.WriteByte(value);
			}
			else if (count <= 65535)
			{
				byte value = 222;
				ms.WriteByte(value);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes((short)count));
				ms.Write(array, 0, array.Length);
			}
			else
			{
				byte value = 223;
				ms.WriteByte(value);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes(count));
				ms.Write(array, 0, array.Length);
			}
			for (int i = 0; i < count; i++)
			{
				WriteTools.WriteString(ms, this.children[i].name);
				this.children[i].Encode2Stream(ms);
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00004BE4 File Offset: 0x00002DE4
		private void WirteArray(Stream ms)
		{
			int count = this.children.Count;
			if (count <= 15)
			{
				byte value = 144 + (byte)count;
				ms.WriteByte(value);
			}
			else if (count <= 65535)
			{
				byte value = 220;
				ms.WriteByte(value);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes((short)count));
				ms.Write(array, 0, array.Length);
			}
			else
			{
				byte value = 221;
				ms.WriteByte(value);
				byte[] array = BytesTools.SwapBytes(BitConverter.GetBytes(count));
				ms.Write(array, 0, array.Length);
			}
			for (int i = 0; i < count; i++)
			{
				this.children[i].Encode2Stream(ms);
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000253F File Offset: 0x0000073F
		public void SetAsInteger(long value)
		{
			this.innerValue = value;
			this.valueType = MsgPackType.Integer;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00002554 File Offset: 0x00000754
		public void SetAsUInt64(ulong value)
		{
			this.innerValue = value;
			this.valueType = MsgPackType.UInt64;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004C98 File Offset: 0x00002E98
		public ulong GetAsUInt64()
		{
			switch (this.valueType)
			{
			case MsgPackType.String:
				return ulong.Parse(this.innerValue.ToString().Trim());
			case MsgPackType.Integer:
				return Convert.ToUInt64((long)this.innerValue);
			case MsgPackType.UInt64:
				return (ulong)this.innerValue;
			case MsgPackType.Float:
				return Convert.ToUInt64((double)this.innerValue);
			case MsgPackType.Single:
				return Convert.ToUInt64((float)this.innerValue);
			case MsgPackType.DateTime:
				return Convert.ToUInt64((DateTime)this.innerValue);
			}
			return 0UL;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004D40 File Offset: 0x00002F40
		public long GetAsInteger()
		{
			switch (this.valueType)
			{
			case MsgPackType.String:
				return long.Parse(this.innerValue.ToString().Trim());
			case MsgPackType.Integer:
				return (long)this.innerValue;
			case MsgPackType.UInt64:
				return Convert.ToInt64((long)this.innerValue);
			case MsgPackType.Float:
				return Convert.ToInt64((double)this.innerValue);
			case MsgPackType.Single:
				return Convert.ToInt64((float)this.innerValue);
			case MsgPackType.DateTime:
				return Convert.ToInt64((DateTime)this.innerValue);
			}
			return 0L;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004DE8 File Offset: 0x00002FE8
		public double GetAsFloat()
		{
			switch (this.valueType)
			{
			case MsgPackType.String:
				return double.Parse((string)this.innerValue);
			case MsgPackType.Integer:
				return Convert.ToDouble((long)this.innerValue);
			case MsgPackType.Float:
				return (double)this.innerValue;
			case MsgPackType.Single:
				return (double)((float)this.innerValue);
			case MsgPackType.DateTime:
				return (double)Convert.ToInt64((DateTime)this.innerValue);
			}
			return 0.0;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00002569 File Offset: 0x00000769
		public void SetAsBytes(byte[] value)
		{
			this.innerValue = value;
			this.valueType = MsgPackType.Binary;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004E7C File Offset: 0x0000307C
		public byte[] GetAsBytes()
		{
			switch (this.valueType)
			{
			case MsgPackType.String:
				return BytesTools.GetUtf8Bytes(this.innerValue.ToString());
			case MsgPackType.Integer:
				return BitConverter.GetBytes((long)this.innerValue);
			case MsgPackType.Float:
				return BitConverter.GetBytes((double)this.innerValue);
			case MsgPackType.Single:
				return BitConverter.GetBytes((float)this.innerValue);
			case MsgPackType.DateTime:
				return BitConverter.GetBytes(((DateTime)this.innerValue).ToBinary());
			case MsgPackType.Binary:
				return (byte[])this.innerValue;
			}
			return new byte[0];
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000257A File Offset: 0x0000077A
		public void Add(string key, string value)
		{
			MsgPack msgPack = this.InnerAddArrayChild();
			msgPack.name = key;
			msgPack.SetAsString(value);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000258F File Offset: 0x0000078F
		public void Add(string key, int value)
		{
			MsgPack msgPack = this.InnerAddArrayChild();
			msgPack.name = key;
			msgPack.SetAsInteger((long)value);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004F2C File Offset: 0x0000312C
		public bool LoadFileAsBytes(string fileName)
		{
			if (File.Exists(fileName))
			{
				FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				fileStream.Close();
				fileStream.Dispose();
				this.SetAsBytes(array);
				return true;
			}
			return false;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004F84 File Offset: 0x00003184
		public bool SaveBytesToFile(string fileName)
		{
			if (this.innerValue != null)
			{
				FileStream fileStream = new FileStream(fileName, FileMode.Append);
				fileStream.Write((byte[])this.innerValue, 0, ((byte[])this.innerValue).Length);
				fileStream.Close();
				fileStream.Dispose();
				return true;
			}
			return false;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004FD0 File Offset: 0x000031D0
		public MsgPack ForcePathObject(string path)
		{
			MsgPack msgPack = this;
			string[] array = path.Trim().Split(new char[]
			{
				'.',
				'/',
				'\\'
			});
			if (array.Length == 0)
			{
				return null;
			}
			string text;
			if (array.Length > 1)
			{
				for (int i = 0; i < array.Length - 1; i++)
				{
					text = array[i];
					MsgPack msgPack2 = msgPack.FindObject(text);
					if (msgPack2 == null)
					{
						msgPack = msgPack.InnerAddMapChild();
						msgPack.SetName(text);
					}
					else
					{
						msgPack = msgPack2;
					}
				}
			}
			text = array[array.Length - 1];
			int num = msgPack.IndexOf(text);
			if (num > -1)
			{
				return msgPack.children[num];
			}
			msgPack = msgPack.InnerAddMapChild();
			msgPack.SetName(text);
			return msgPack;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000025A5 File Offset: 0x000007A5
		public void SetAsNull()
		{
			this.Clear();
			this.innerValue = null;
			this.valueType = MsgPackType.Null;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000025BB File Offset: 0x000007BB
		public void SetAsString(string value)
		{
			this.innerValue = value;
			this.valueType = MsgPackType.String;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000025CB File Offset: 0x000007CB
		public string GetAsString()
		{
			if (this.innerValue == null)
			{
				return "";
			}
			return this.innerValue.ToString();
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000025E9 File Offset: 0x000007E9
		public void SetAsBoolean(bool bVal)
		{
			this.valueType = MsgPackType.Boolean;
			this.innerValue = bVal;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000025FE File Offset: 0x000007FE
		public void SetAsSingle(float fVal)
		{
			this.valueType = MsgPackType.Single;
			this.innerValue = fVal;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00002614 File Offset: 0x00000814
		public void SetAsFloat(double fVal)
		{
			this.valueType = MsgPackType.Float;
			this.innerValue = fVal;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000508C File Offset: 0x0000328C
		public void DecodeFromBytes(byte[] bytes)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bytes = Zip.Decompress(bytes);
				memoryStream.Write(bytes, 0, bytes.Length);
				memoryStream.Position = 0L;
				this.DecodeFromStream(memoryStream);
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000050E4 File Offset: 0x000032E4
		public void DecodeFromFile(string fileName)
		{
			FileStream fileStream = new FileStream(fileName, FileMode.Open);
			this.DecodeFromStream(fileStream);
			fileStream.Dispose();
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00005108 File Offset: 0x00003308
		public void DecodeFromStream(Stream ms)
		{
			byte b = (byte)ms.ReadByte();
			if (b <= 127)
			{
				this.SetAsInteger((long)((ulong)b));
				return;
			}
			if (b >= 128 && b <= 143)
			{
				this.Clear();
				this.valueType = MsgPackType.Map;
				int num = (int)(b - 128);
				for (int i = 0; i < num; i++)
				{
					MsgPack msgPack = this.InnerAdd();
					msgPack.SetName(ReadTools.ReadString(ms));
					msgPack.DecodeFromStream(ms);
				}
				return;
			}
			if (b >= 144 && b <= 159)
			{
				this.Clear();
				this.valueType = MsgPackType.Array;
				int num = (int)(b - 144);
				for (int i = 0; i < num; i++)
				{
					this.InnerAdd().DecodeFromStream(ms);
				}
				return;
			}
			if (b >= 160 && b <= 191)
			{
				int num = (int)(b - 160);
				this.SetAsString(ReadTools.ReadString(ms, num));
				return;
			}
			if (b >= 224 && b <= 255)
			{
				this.SetAsInteger((long)((sbyte)b));
				return;
			}
			if (b == 192)
			{
				this.SetAsNull();
				return;
			}
			if (b == 193)
			{
				throw new Exception("(never used) type $c1");
			}
			if (b == 194)
			{
				this.SetAsBoolean(false);
				return;
			}
			if (b == 195)
			{
				this.SetAsBoolean(true);
				return;
			}
			if (b == 196)
			{
				int num = ms.ReadByte();
				byte[] array = new byte[num];
				ms.Read(array, 0, num);
				this.SetAsBytes(array);
				return;
			}
			if (b == 197)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToUInt16(array, 0);
				array = new byte[num];
				ms.Read(array, 0, num);
				this.SetAsBytes(array);
				return;
			}
			if (b == 198)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				int num = BitConverter.ToInt32(array, 0);
				array = new byte[num];
				ms.Read(array, 0, num);
				this.SetAsBytes(array);
				return;
			}
			if (b == 199 || b == 200 || b == 201)
			{
				throw new Exception("(ext8,ext16,ex32) type $c7,$c8,$c9");
			}
			if (b == 202)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				this.SetAsSingle(BitConverter.ToSingle(array, 0));
				return;
			}
			if (b == 203)
			{
				byte[] array = new byte[8];
				ms.Read(array, 0, 8);
				array = BytesTools.SwapBytes(array);
				this.SetAsFloat(BitConverter.ToDouble(array, 0));
				return;
			}
			if (b == 204)
			{
				b = (byte)ms.ReadByte();
				this.SetAsInteger((long)((ulong)b));
				return;
			}
			if (b == 205)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger((long)((ulong)BitConverter.ToUInt16(array, 0)));
				return;
			}
			if (b == 206)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger((long)((ulong)BitConverter.ToUInt32(array, 0)));
				return;
			}
			if (b == 207)
			{
				byte[] array = new byte[8];
				ms.Read(array, 0, 8);
				array = BytesTools.SwapBytes(array);
				this.SetAsUInt64(BitConverter.ToUInt64(array, 0));
				return;
			}
			if (b == 220)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToInt16(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Array;
				for (int i = 0; i < num; i++)
				{
					this.InnerAdd().DecodeFromStream(ms);
				}
				return;
			}
			if (b == 221)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToInt16(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Array;
				for (int i = 0; i < num; i++)
				{
					this.InnerAdd().DecodeFromStream(ms);
				}
				return;
			}
			if (b == 217)
			{
				this.SetAsString(ReadTools.ReadString(b, ms));
				return;
			}
			if (b == 222)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToInt16(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Map;
				for (int i = 0; i < num; i++)
				{
					MsgPack msgPack2 = this.InnerAdd();
					msgPack2.SetName(ReadTools.ReadString(ms));
					msgPack2.DecodeFromStream(ms);
				}
				return;
			}
			if (b == 222)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				int num = (int)BitConverter.ToInt16(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Map;
				for (int i = 0; i < num; i++)
				{
					MsgPack msgPack3 = this.InnerAdd();
					msgPack3.SetName(ReadTools.ReadString(ms));
					msgPack3.DecodeFromStream(ms);
				}
				return;
			}
			if (b == 223)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				int num = BitConverter.ToInt32(array, 0);
				this.Clear();
				this.valueType = MsgPackType.Map;
				for (int i = 0; i < num; i++)
				{
					MsgPack msgPack4 = this.InnerAdd();
					msgPack4.SetName(ReadTools.ReadString(ms));
					msgPack4.DecodeFromStream(ms);
				}
				return;
			}
			if (b == 218)
			{
				this.SetAsString(ReadTools.ReadString(b, ms));
				return;
			}
			if (b == 219)
			{
				this.SetAsString(ReadTools.ReadString(b, ms));
				return;
			}
			if (b == 208)
			{
				this.SetAsInteger((long)((sbyte)ms.ReadByte()));
				return;
			}
			if (b == 209)
			{
				byte[] array = new byte[2];
				ms.Read(array, 0, 2);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger((long)BitConverter.ToInt16(array, 0));
				return;
			}
			if (b == 210)
			{
				byte[] array = new byte[4];
				ms.Read(array, 0, 4);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger((long)BitConverter.ToInt32(array, 0));
				return;
			}
			if (b == 211)
			{
				byte[] array = new byte[8];
				ms.Read(array, 0, 8);
				array = BytesTools.SwapBytes(array);
				this.SetAsInteger(BitConverter.ToInt64(array, 0));
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000571C File Offset: 0x0000391C
		public byte[] Encode2Bytes()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.Encode2Stream(memoryStream);
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Position = 0L;
				memoryStream.Read(array, 0, (int)memoryStream.Length);
				result = Zip.Compress(array);
			}
			return result;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005788 File Offset: 0x00003988
		public void Encode2Stream(Stream ms)
		{
			switch (this.valueType)
			{
			case MsgPackType.Unknown:
			case MsgPackType.Null:
				WriteTools.WriteNull(ms);
				return;
			case MsgPackType.Map:
				this.WriteMap(ms);
				return;
			case MsgPackType.Array:
				this.WirteArray(ms);
				return;
			case MsgPackType.String:
				WriteTools.WriteString(ms, (string)this.innerValue);
				return;
			case MsgPackType.Integer:
				WriteTools.WriteInteger(ms, (long)this.innerValue);
				return;
			case MsgPackType.UInt64:
				WriteTools.WriteUInt64(ms, (ulong)this.innerValue);
				return;
			case MsgPackType.Boolean:
				WriteTools.WriteBoolean(ms, (bool)this.innerValue);
				return;
			case MsgPackType.Float:
				WriteTools.WriteFloat(ms, (double)this.innerValue);
				return;
			case MsgPackType.Single:
				WriteTools.WriteFloat(ms, (double)((float)this.innerValue));
				return;
			case MsgPackType.DateTime:
				WriteTools.WriteInteger(ms, this.GetAsInteger());
				return;
			case MsgPackType.Binary:
				WriteTools.WriteBinary(ms, (byte[])this.innerValue);
				return;
			default:
				WriteTools.WriteNull(ms);
				return;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00002629 File Offset: 0x00000829
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00002631 File Offset: 0x00000831
		public string AsString
		{
			get
			{
				return this.GetAsString();
			}
			set
			{
				this.SetAsString(value);
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000263A File Offset: 0x0000083A
		// (set) Token: 0x060000AD RID: 173 RVA: 0x00002642 File Offset: 0x00000842
		public long AsInteger
		{
			get
			{
				return this.GetAsInteger();
			}
			set
			{
				this.SetAsInteger(value);
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000AE RID: 174 RVA: 0x0000264B File Offset: 0x0000084B
		// (set) Token: 0x060000AF RID: 175 RVA: 0x00002653 File Offset: 0x00000853
		public double AsFloat
		{
			get
			{
				return this.GetAsFloat();
			}
			set
			{
				this.SetAsFloat(value);
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00005880 File Offset: 0x00003A80
		public MsgPackArray AsArray
		{
			get
			{
				lock (this)
				{
					if (this.refAsArray == null)
					{
						this.refAsArray = new MsgPackArray(this, this.children);
					}
				}
				return this.refAsArray;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x0000265C File Offset: 0x0000085C
		public MsgPackType ValueType
		{
			get
			{
				return this.valueType;
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00002664 File Offset: 0x00000864
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new MsgPackEnum(this.children);
		}

		// Token: 0x04000047 RID: 71
		private string name;

		// Token: 0x04000048 RID: 72
		private string lowerName;

		// Token: 0x04000049 RID: 73
		private object innerValue;

		// Token: 0x0400004A RID: 74
		private MsgPackType valueType;

		// Token: 0x0400004B RID: 75
		private MsgPack parent;

		// Token: 0x0400004C RID: 76
		private List<MsgPack> children = new List<MsgPack>();

		// Token: 0x0400004D RID: 77
		private MsgPackArray refAsArray;
	}
}
