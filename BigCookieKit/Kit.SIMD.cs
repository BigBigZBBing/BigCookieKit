using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;
using System.IO;

namespace BigCookieKit
{
    public static partial class Kit
    {
        /// <summary>
        /// 连续数组中判断连续结果集的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int FastIndexOf<T>(this T[] array, params T[] target) where T : struct
        {
            var count = Vector<T>.Count;
            var vectorValue = new Vector<T>(target[0]);
            if (target.Length > count) throw new ArgumentOutOfRangeException();
            int index = 0;
            do
            {
                var t11 = new Vector<T>(ArrayComplement(array, count), index);
                if (Vector.EqualsAny(t11, vectorValue))
                {
                    var temp = array[index..count];
                    for (int t = 0; t < temp.Length; t++)
                    {
                        if (temp[t].Equals(target[0]))
                        {
                            var t1 = new Vector<T>(ArrayComplement(array[(t + index)..target.Length], count));
                            var t2 = new Vector<T>(ArrayComplement(target, count));
                            if (Vector.EqualsAll(t1, t2)) return t + index;
                        }
                    }
                }
                index += count;
            } while (index < array.Length);
            return -1;
        }

        private static T[] ArrayComplement<T>(T[] source, int length) where T : struct
        {
            T[] res = new T[length];
            if (source.Length > length)
            {
                source = source.AsSpan(0, length).ToArray();
            }
            Array.Copy(source, res, source.Length);
            return res;
        }
    }
}
