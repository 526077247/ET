using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
	public class MessagerIdComparer : IEqualityComparer<MessagerId>
	{
		public static MessagerIdComparer Instance = new MessagerIdComparer();
		public bool Equals(MessagerId x, MessagerId y)
		{
			return x == y;          //x.Equals(y);  注意这里不要使用Equals方法，因为也会造成封箱操作
		}

		public int GetHashCode(MessagerId x)
		{
			return (int)x;
		}
	}
	public enum MessagerId:byte
    {
        OnLanguageChange,//多语言切换
        OnClickUnit,
    }

}
