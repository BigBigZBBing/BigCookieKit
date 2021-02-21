using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILWheatBread.SmartCompress
{
    class Test
    {
        #region Int16位算法
        //public static Byte[] GetBytes(Int16 value)
        //{
        //    Byte[] bytes = new Byte[2];
        //    bytes[0] = (Byte)(value & 0xFF);
        //    bytes[1] = (Byte)((value >> 8) & 0xFF);
        //}
        #endregion

        #region Int32位算法
        //public static Byte[] GetBytes(Int32 value)
        //{
        //    Byte[] buffer = new Byte[4];
        //    for (int index = 0; index < buffer.Length; index++)
        //    {
        //        buffer[index] = (Byte)((value >> (8 * index)) & 0xFF);
        //    }
        //    return buffer;
        //}
        #endregion

        #region Int32循环指针算法
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public unsafe static Byte[] GetBytes(Int32 value)
        //{
        //    Byte i;
        //    Byte[] bytes = new Byte[sizeof(Int32)];
        //    fixed (Byte* arr = bytes)
        //    {
        //        Byte* pdata = (Byte*)&value;
        //        for (i = 0; i < sizeof(Int32); ++i)
        //        {
        //            *(arr + i) = *(pdata + i);
        //        }
        //    }
        //    return bytes;
        //}
        #endregion

        #region Int64位算法
        //public static Byte[] GetBytes(Int64 value)
        //{
        //    Byte[] buffer = new Byte[8];
        //    for (int index = 0; index < buffer.Length; index++)
        //    {
        //        buffer[index] = (Byte)((value >> (8 * index)) & 0xFF);
        //    }
        //    return buffer;
        //}
        #endregion

        #region Int64指针循环算法
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public unsafe static Byte[] GetBytes(Int64 value)
        //{
        //    Byte i;
        //    Byte[] bytes = new Byte[sizeof(Int64)];
        //    fixed (Byte* arr = bytes)
        //    {
        //        Byte* pdata = (Byte*)&value;
        //        for (i = 0; i < sizeof(Int64); ++i)
        //        {
        //            *(arr + i) = *(pdata + i);
        //        }
        //    }
        //    return bytes;
        //}
        #endregion

        #region Single指针循环算法
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public unsafe static Byte[] GetBytes(Single value)
        //{
        //    Byte i;
        //    Byte[] bytes = new Byte[sizeof(Single)];
        //    fixed (Byte* arr = bytes)
        //    {
        //        Byte* pdata = (Byte*)&value;
        //        for (i = 0; i < sizeof(Single); ++i)
        //        {
        //            *(arr + i) = *(pdata + i);
        //        }
        //    }
        //    return bytes;
        //}
        #endregion

        //public static Int16 BitToInt16(Byte[] bytes)
        //{
        //    Int16 value;
        //    value = (Int16)((bytes[0] & 0xFF)
        //            | ((bytes[1] & 0xFF) << 8));
        //    return value;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private unsafe static Int32 BitToInt32(Byte[] data)
        //{
        //    Byte i;
        //    Int32 value = 0;
        //    fixed (Byte* @ref = data)
        //    {
        //        Byte* val = (Byte*)&value;
        //        for (i = 0; i < data.Length; i++)
        //        {
        //            *(val + i) = *(@ref + i);
        //        }
        //    }
        //    return value;
        //}

        //public static Int64 BitToInt64(Byte[] bytes)
        //{
        //    UInt64 value = 0;
        //    for (int index = 0; index < bytes.Length; index++)
        //    {
        //        value |= (UInt64)(bytes[index] & 0xFF) << (8 * index);
        //    }
        //    return (Int64)value;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public unsafe static Int64 BitToInt64(Byte[] data)
        //{
        //    Byte i;
        //    Int64 value = 0;
        //    fixed (Byte* @ref = data)
        //    {
        //        Byte* val = (Byte*)&value;
        //        for (i = 0; i < data.Length; i++)
        //        {
        //            *(val + i) = *(@ref + i);
        //        }
        //    }
        //    return value;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public unsafe static Single BitToSingle(Byte[] data)
        //{
        //    Byte i;
        //    Single value = 0.0F;
        //    fixed (Byte* @ref = data)
        //    {
        //        Byte* val = (Byte*)&value;
        //        for (i = 0; i < data.Length; i++)
        //        {
        //            *(val + i) = *(@ref + i);
        //        }
        //    }
        //    return value;
        //}
    }
}
