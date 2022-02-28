using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BigCookieKit
{
    public static partial class Kit
    {
        public static byte[] GetBytes(Char[] value)
        {
            int i;
            List<byte> bytes = new List<byte>();
            for (i = 0; i < value.Length; i++)
            {
                bytes.AddRange(GetBytes(value[i]));
            }
            return bytes.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] GetBytes(Char value)
        {
            UInt32 ChLen = 1;
            if (value >= SByte.MaxValue) ChLen = 2;
            byte[] bytes = new byte[ChLen];
            fixed (byte* buf = bytes)
            {
                *(Char*)buf = value;
            }
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] GetBytes(Int16 value)
        {
            byte[] bytes = new byte[2];
            fixed (byte* buf = bytes)
            {
                *(Int16*)buf = value;
            }
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] GetBytes(int value)
        {
            byte[] bytes = new byte[4];
            fixed (byte* buf = bytes)
            {
                *(int*)buf = value;
            }
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] GetBytes(long value)
        {
            byte[] bytes = new byte[8];
            fixed (byte* b = bytes)
            {
                *((long*)b) = value;
            }
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] GetBytes(float value)
        {
            byte[] bytes = new byte[4];
            fixed (byte* b = bytes)
            {
                *((float*)b) = value;
            }
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] GetBytes(double value)
        {
            byte[] bytes = new byte[8];
            fixed (byte* b = bytes)
            {
                *((double*)b) = value;
            }
            return bytes;
        }

        public static byte[] GetBytes(decimal value)
        {
            int i;
            List<byte> bytes = new List<byte>();
            int[] bits = decimal.GetBits(value);
            for (i = 0; i < bits.Length; i++)
            {
                bytes.AddRange(GetBytes(bits[i]));
            }
            return bytes.ToArray();
        }

        public static string BitToString(byte[] data, Encoding encoding = default)
        {
            return (encoding ?? Encoding.UTF8).GetString(data);
        }

        public static Char[] BitToCharArray(byte[] data)
        {
            return Encoding.UTF8.GetChars(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Char BitToChar(byte[] data)
        {
            fixed (byte* pbyte = &data[0])
            {
                return *(Char*)pbyte;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Int16 BitToInt16(byte[] data)
        {
            fixed (byte* pbyte = &data[0])
            {
                return *(Int16*)pbyte;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int BitToInt32(byte[] data)
        {
            fixed (byte* pbyte = &data[0])
            {
                return *(int*)pbyte;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long BitToInt64(byte[] data)
        {
            fixed (byte* pbyte = &data[0])
            {
                return *(long*)pbyte;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float BitToSingle(byte[] data)
        {
            fixed (byte* pbyte = &data[0])
            {
                return *(float*)pbyte;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double BitToDouble(byte[] data)
        {
            fixed (byte* pbyte = &data[0])
            {
                return *(double*)pbyte;
            }
        }

        public static decimal BitToDecimal(byte[] data)
        {
            return new decimal(new int[] {
                BitToInt32(data.AsSpan(0, 4).ToArray()),
                BitToInt32(data.AsSpan(4, 4).ToArray()),
                BitToInt32(data.AsSpan(8, 4).ToArray()),
                BitToInt32(data.AsSpan(12, 4).ToArray()),
            });
        }
    }
}