//#define SIMD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
#if NETCORE5
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
#endif

namespace ILWheatBread.Optimizing
{
    public static partial class BasicOptimize
    {

#if NETCORE5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void CopyIntrinsics(void* arr1, void* arr2, Int32 count)
        {
            Int32 i;
            Vector256<Byte> v1;
            Int32 vectorSize = Vector256<Byte>.Count;
            Byte* pbyte1 = (Byte*)arr1;
            Byte* pbyte2 = (Byte*)arr2;
            for (i = 0; i < count; i += vectorSize)
            {
                v1 = Avx2.LoadVector256((pbyte1 + i));
                Avx2.Store((pbyte2 + i), v1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TCopy<T>(T[] arr1, T[] arr2, Int32 count) where T : struct
        {
            Int32 i;
            Vector<T> v1;
            Int32 vectorSize = Vector<T>.Count;
            if (count >= vectorSize)
            {
                Int32 rest = count % vectorSize;
                Int32 length = count - rest;
                for (i = 0; i < length; i += vectorSize)
                {
                    v1 = new Vector<T>(arr1, i);
                    v1.CopyTo(arr2, i);
                }

                if (rest != 0)
                    Buffer.BlockCopy(arr1, length, arr2, length, rest);
            }
            else
            {
                Buffer.BlockCopy(arr1, 0, arr2, 0, count);
            }
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(Char[] arr1, Char[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (Char* pbyte1 = arr1)
                fixed (Char* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(SByte[] arr1, SByte[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (SByte* pbyte1 = arr1)
                fixed (SByte* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(Byte[] arr1, Byte[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (Byte* pbyte1 = arr1)
                fixed (Byte* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(Int16[] arr1, Int16[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (Int16* pbyte1 = arr1)
                fixed (Int16* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(Int32[] arr1, Int32[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (Int32* pbyte1 = arr1)
                fixed (Int32* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(Int64[] arr1, Int64[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (Int64* pbyte1 = arr1)
                fixed (Int64* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(UInt16[] arr1, UInt16[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (UInt16* pbyte1 = arr1)
                fixed (UInt16* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(UInt32[] arr1, UInt32[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (UInt32* pbyte1 = arr1)
                fixed (UInt32* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(UInt64[] arr1, UInt64[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (UInt64* pbyte1 = arr1)
                fixed (UInt64* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(Single[] arr1, Single[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (Single* pbyte1 = arr1)
                fixed (Single* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(Double[] arr1, Double[] arr2, Int32 count)
        {
#if SIMD
            TCopy(arr1, arr2, count);
#else
#if NETCORE5
            unsafe
            {
                fixed (Double* pbyte1 = arr1)
                fixed (Double* pbyte2 = arr2)
                {
                    CopyIntrinsics(pbyte1, pbyte2, count);
                }
            }
#endif
#endif
        }

    }
}
