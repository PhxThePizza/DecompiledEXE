using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Client.Connection
{
	// Token: 0x02000007 RID: 7
	internal class Win32
	{
		// Token: 0x0600002C RID: 44
		[DllImport("kernel32", SetLastError = true)]
		public static extern IntPtr LoadLibraryA([MarshalAs(UnmanagedType.VBByRefStr)] ref string Name);

		// Token: 0x0600002D RID: 45
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hProcess, [MarshalAs(UnmanagedType.VBByRefStr)] ref string Name);

		// Token: 0x0600002E RID: 46 RVA: 0x000021A1 File Offset: 0x000003A1
		public static CreateApi LoadApi<CreateApi>(string name, string method)
		{
			return (!!0)((object)Marshal.GetDelegateForFunctionPointer(Win32.GetProcAddress(Win32.LoadLibraryA(ref name), ref method), typeof(!!0)));
		}

		// Token: 0x04000021 RID: 33
		public static readonly Win32.DelegateVirtualProtect VirtualAllocEx = Win32.LoadApi<Win32.DelegateVirtualProtect>("kernel32", Encoding.Default.GetString(Convert.FromBase64String("VmlydHVhbFByb3RlY3Q=")));

		// Token: 0x02000008 RID: 8
		// (Invoke) Token: 0x06000032 RID: 50
		public delegate int DelegateVirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
	}
}
