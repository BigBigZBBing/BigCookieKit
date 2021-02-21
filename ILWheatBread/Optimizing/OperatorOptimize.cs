using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ILWheatBread.Optimizing
{
    public static partial class BasicOptimize
    {
        /// <summary>
        /// 加法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Add(long left, long right)
        {
            //表示两个二进制 每个位的和后的结果 此步骤只在二进制有代表性
            long sum = left ^ right;
            //表示是否有进位
            long carry = left & right;
            //全是0就是没进位
            while (carry != 0)
            {
                //如果有进位 往前推一位手工进位 接下来以此往复
                carry <<= 1;
                long temp = sum;
                sum ^= carry;
                carry = temp & carry;
            }
            return sum;
        }

        /// <summary>
        /// 加法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Add(int left, int right)
        {
            return (int)Add((long)left, (long)right);
        }

        /// <summary>
        /// 减法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sub(long left, long right)
        {
            //减法就是 取反补位后 的 加法
            right = ~right + 1;
            long sum = left ^ right;
            long carry = left & right;
            while (carry != 0)
            {
                carry <<= 1;
                long temp = sum;
                sum ^= carry;
                carry = temp & carry;
            }
            return sum;
        }

        /// <summary>
        /// 减法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sub(int left, int right)
        {
            return (int)Sub((long)left, (long)right);
        }

        /// <summary>
        /// 乘法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Mul(long left, long right)
        {
            //首先判断是否为2的幂
            if (right > 1 && IsPower(right))
                return left << PowerNum(right);

            int i = 0;
            long res = 0;
            //乘数为零为终止条件
            while (right != 0)
            {
                //如果乘数为奇数
                if ((right & 1) == 1)
                {
                    res = res + (left << i);
                    //乘数左移一位
                    right = right >> 1;
                    //记录移动的位数
                    i = Add(i, 1);
                }
                else
                {
                    //如果为0,那么就不加
                    right = right >> 1;
                    //记录移动的位数s
                    i = Add(i, 1);
                }
            }
            return res;
        }

        /// <summary>
        /// 乘法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mul(int left, int right)
        {
            return (int)Mul((long)left, (long)right);
        }

        /// <summary>
        /// 除法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Div(long left, long right)
        {
            //首先判断是否为2的幂
            if (right > 1 && IsPower(right))
                return left >> PowerNum(right);

            long res = 0;
            //求绝对值
            long temp = left < 0 ? ~left + 1 : left;
            while (true)
            {
                if (temp < right)
                {
                    //如果被除数是负数 则把结果取反
                    if (left < 0)
                        return ~res + 1;
                    return res;
                }
                temp = Sub(temp, right);
                res = Add(res, 1);
            }
        }

        /// <summary>
        /// 除法
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Div(int left, int right)
        {
            return (int)Div((long)left, (long)right);
        }

        /// <summary>
        /// 是否为奇数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOdd(int num)
        {
            return (num & 1) == 1;
        }

        /// <summary>
        /// 是否为偶数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(int num)
        {
            return (num & 1) == 0;
        }

        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equ(int left, int right)
        {
            return ~(left - right | right - left) == 0;
        }

        /// <summary>
        /// 是否为2的N次幂
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPower(long num)
        {
            return ((num & (num - 1)) == 0) && (num != 0);
        }

        /// <summary>
        /// 求出数字为2的幂数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PowerNum(long num)
        {
            int a = 1;
            while (num == (2 << a))
            {
                if ((2 << a) > num)
                    return 0;
                a = Add(a, 1);
            }
            return a;
        }

        /// <summary>
        /// 返回平均值
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ave(int left, int right)
        {
            return (left & right) + ((left ^ right) >> 1);
        }
    }
}
