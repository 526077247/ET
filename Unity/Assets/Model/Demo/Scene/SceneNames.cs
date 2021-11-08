using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class SceneNamesCompare : IEqualityComparer<SceneNames>
	{
		public static SceneNamesCompare Instance = new SceneNamesCompare();
		public bool Equals(SceneNames x, SceneNames y)
		{
			return x == y;          //x.Equals(y);  注意这里不要使用Equals方法，因为也会造成封箱操作
		}

		public int GetHashCode(SceneNames x)
		{
			return (int)x;
		}
	}
	public enum SceneNames:byte
    {
		None,
        Init,
        Loading,
        Login,
        Map,
    }
}
