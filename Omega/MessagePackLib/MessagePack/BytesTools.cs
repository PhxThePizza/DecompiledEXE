using System;
using System.Text;

namespace MessagePackLib.MessagePack
{
	// Token: 0x0200001B RID: 27
	public class BytesTools
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00002394 File Offset: 0x00000594
		public static byte[] GetUtf8Bytes(string s)
		{
			return BytesTools.utf8Encode.GetBytes(s);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000023A1 File Offset: 0x000005A1
		public static string GetString(byte[] utf8Bytes)
		{
			return BytesTools.utf8Encode.GetString(utf8Bytes);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004940 File Offset: 0x00002B40
		public static string BytesAsString(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				stringBuilder.Append(string.Format("{0:D3} ", b));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004988 File Offset: 0x00002B88
		public static string BytesAsHexString(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				stringBuilder.Append(string.Format("{0:X2} ", b));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000049D0 File Offset: 0x00002BD0
		public static byte[] SwapBytes(byte[] v)
		{
			byte[] array = new byte[v.Length];
			int num = v.Length - 1;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = v[num];
				num--;
			}
			return array;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000023AE File Offset: 0x000005AE
		public static byte[] SwapInt64(long v)
		{
			return BytesTools.SwapBytes(BitConverter.GetBytes(v));
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000023BB File Offset: 0x000005BB
		public static byte[] SwapInt32(int v)
		{
			byte[] array = new byte[]
			{
				0,
				0,
				0,
				(byte)v
			};
			array[2] = (byte)(v >> 8);
			array[1] = (byte)(v >> 16);
			array[0] = (byte)(v >> 24);
			return array;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000023DF File Offset: 0x000005DF
		public static byte[] SwapInt16(short v)
		{
			byte[] array = new byte[]
			{
				0,
				(byte)v
			};
			array[0] = (byte)(v >> 8);
			return array;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000023F3 File Offset: 0x000005F3
		public static byte[] SwapDouble(double v)
		{
			return BytesTools.SwapBytes(BitConverter.GetBytes(v));
		}

		// Token: 0x04000042 RID: 66
		private static UTF8Encoding utf8Encode = new UTF8Encoding();
	}
}
