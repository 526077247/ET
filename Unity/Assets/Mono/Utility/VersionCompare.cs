using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class VersionCompare
    {
        public static int Compare(string n1,string n2)
        {
            if (string.IsNullOrEmpty(n1) || string.IsNullOrEmpty(n2)) return 0;
            string[] b1 = n1.Split('.');
            string[] b2 = n2.Split('.');
            if (b1.Length != b2.Length) return b1.Length > b2.Length ? 1 : -1;
            for (int i = 0; i < b1.Length; i++)
            {
                var d1 = float.Parse("0." + b1[i]);
                var d2 = float.Parse("0." + b2[i]);
                if (d1 != d2)
                {
                    return d1 > d2 ? 1 : -1;
                }
            }
            return 0;
        }
    }
}
