using System;
using System.Collections;
using System.Collections.Generic;

namespace MessagePackLib.MessagePack
{
	// Token: 0x0200001C RID: 28
	public class MsgPackEnum : IEnumerator
	{
		// Token: 0x0600007E RID: 126 RVA: 0x0000240C File Offset: 0x0000060C
		public MsgPackEnum(List<MsgPack> obj)
		{
			this.children = obj;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00002422 File Offset: 0x00000622
		object IEnumerator.Current
		{
			get
			{
				return this.children[this.position];
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00002435 File Offset: 0x00000635
		bool IEnumerator.MoveNext()
		{
			this.position++;
			return this.position < this.children.Count;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002458 File Offset: 0x00000658
		void IEnumerator.Reset()
		{
			this.position = -1;
		}

		// Token: 0x04000043 RID: 67
		private List<MsgPack> children;

		// Token: 0x04000044 RID: 68
		private int position = -1;
	}
}
