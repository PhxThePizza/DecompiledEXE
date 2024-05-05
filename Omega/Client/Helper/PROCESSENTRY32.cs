using System;
using System.Runtime.InteropServices;

namespace Client.Helper
{
	// Token: 0x0200000B RID: 11
	public struct PROCESSENTRY32
	{
		// Token: 0x04000024 RID: 36
		public uint dwSize;

		// Token: 0x04000025 RID: 37
		public uint cntUsage;

		// Token: 0x04000026 RID: 38
		public uint th32ProcessID;

		// Token: 0x04000027 RID: 39
		public IntPtr th32DefaultHeapID;

		// Token: 0x04000028 RID: 40
		public uint th32ModuleID;

		// Token: 0x04000029 RID: 41
		public uint cntThreads;

		// Token: 0x0400002A RID: 42
		public uint th32ParentProcessID;

		// Token: 0x0400002B RID: 43
		public int pcPriClassBase;

		// Token: 0x0400002C RID: 44
		public uint dwFlags;

		// Token: 0x0400002D RID: 45
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szExeFile;
	}
}
