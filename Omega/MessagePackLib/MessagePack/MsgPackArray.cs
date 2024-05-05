using System;
using System.Collections.Generic;

namespace MessagePackLib.MessagePack
{
	// Token: 0x0200001D RID: 29
	public class MsgPackArray
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00002461 File Offset: 0x00000661
		public MsgPackArray(MsgPack msgpackObj, List<MsgPack> listObj)
		{
			this.owner = msgpackObj;
			this.children = listObj;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00002477 File Offset: 0x00000677
		public MsgPack Add()
		{
			return this.owner.AddArrayChild();
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00002484 File Offset: 0x00000684
		public MsgPack Add(string value)
		{
			MsgPack msgPack = this.owner.AddArrayChild();
			msgPack.AsString = value;
			return msgPack;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00002498 File Offset: 0x00000698
		public MsgPack Add(long value)
		{
			MsgPack msgPack = this.owner.AddArrayChild();
			msgPack.SetAsInteger(value);
			return msgPack;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000024AC File Offset: 0x000006AC
		public MsgPack Add(double value)
		{
			MsgPack msgPack = this.owner.AddArrayChild();
			msgPack.SetAsFloat(value);
			return msgPack;
		}

		// Token: 0x1700000E RID: 14
		public MsgPack this[int index]
		{
			get
			{
				return this.children[index];
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000088 RID: 136 RVA: 0x000024CE File Offset: 0x000006CE
		public int Length
		{
			get
			{
				return this.children.Count;
			}
		}

		// Token: 0x04000045 RID: 69
		private List<MsgPack> children;

		// Token: 0x04000046 RID: 70
		private MsgPack owner;
	}
}
