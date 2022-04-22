using System;
namespace ET
{
    public static class AOIHelper
    {
        public static long CreateCellId(int x, int y)
        {
            return (long) ((ulong) x << 32) | (uint) y;
        }
        
        public static void KSsort<T>(this ListComponent<T> a, Func<T, T, int> compare, int start = -1, int end = -1)
        {
            if (start < 0) start = 0;
            if (end < 0) end = a.Count - 1;
            bool flag = true;
            T temp;
            while (end != start)
            {
                if (flag)
                {
                    int tempend = a.Count - 1;
                    while (start < tempend)
                    {
                        if (compare(a[start] , a[tempend])>0)//右侧找比自己小的数
                        {
                            temp = a[start];
                            a[start] = a[tempend];
                            a[tempend] = temp;
                            flag = false;
                            break;
                        }
                        else
                        {
                            tempend--;
                            if (start == tempend)
                            {
                                start++;
                                flag = false;
                            }
                        }
                    }
                }
                else
                {
                    int tempstart = 0;
                    while (tempstart < end)
                    {
                        if (compare(a[tempstart], a[end]) > 0)//右侧找比自己小的数
                        {
                            temp = a[tempstart];
                            a[tempstart] = a[end];
                            a[end] = temp;
                            flag = true;
                            break;
                        }
                        else
                        {
                            tempstart++;
                            if (tempstart == end)
                            {
                                end--;
                                flag = true;
                            }
                        }
                    }
                }
            }
        }
    }
}