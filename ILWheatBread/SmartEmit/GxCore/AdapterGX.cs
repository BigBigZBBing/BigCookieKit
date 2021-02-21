using System;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread.SmartEmit
{
    internal static partial class ManagerGX
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EmitValue<T>(this EmitBasic basic, T value)
        {
            if (value == null)
            {
                basic.Emit(OpCodes.Ldnull);
            }
            else if (value.GetType() == typeof(String))
            {
                basic.Emit(OpCodes.Ldstr, Convert.ToString(value));
            }
            else if (value.GetType() == typeof(Boolean))
            {
                switch (Convert.ToBoolean(value))
                {
                    case true: basic.Emit(OpCodes.Ldc_I4_1); break;
                    case false: basic.Emit(OpCodes.Ldc_I4_0); break;
                    default: throw new Exception("boolean to error!");
                }
            }
            else if (value.GetType() == typeof(SByte))
            {
                basic.IntegerMap(Convert.ToSByte(value));
            }
            else if (value.GetType() == typeof(Byte))
            {
                basic.IntegerMap((SByte)Convert.ToByte(value));
            }
            else if (value.GetType() == typeof(Int16))
            {
                basic.IntegerMap(Convert.ToInt16(value));
            }
            else if (value.GetType() == typeof(UInt16))
            {
                basic.IntegerMap((Int16)Convert.ToUInt16(value));
            }
            else if (value.GetType() == typeof(Int32))
            {
                basic.IntegerMap(Convert.ToInt32(value));
            }
            else if (value.GetType() == typeof(UInt32))
            {
                basic.IntegerMap((Int32)Convert.ToUInt32(value));
            }
            else if (value.GetType() == typeof(Int64))
            {
                basic.IntegerMap(Convert.ToInt64(value));
            }
            else if (value.GetType() == typeof(UInt64))
            {
                basic.IntegerMap((Int64)Convert.ToUInt64(value));
            }
            else if (value.GetType() == typeof(Single))
            {
                basic.Emit(OpCodes.Ldc_R4, Convert.ToSingle(value));
            }
            else if (value.GetType() == typeof(Double))
            {
                basic.Emit(OpCodes.Ldc_R8, Convert.ToDouble(value));
            }
            else if (value.GetType() == typeof(Decimal))
            {
                Int32[] bits = Decimal.GetBits(Convert.ToDecimal(value));
                basic.IntegerMap(bits[0]);
                basic.IntegerMap(bits[1]);
                basic.IntegerMap(bits[2]);
                basic.Emit((bits[3] & 0x80000000) != 0 ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                basic.IntegerMap((bits[3] >> 16) & 0x7f);
                basic.Emit(OpCodes.Newobj, typeof(Decimal)
                    .GetConstructor(new Type[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Boolean), typeof(Byte) }));
            }
            else if (value.GetType() == typeof(DateTime))
            {
                basic.Emit(OpCodes.Ldc_I8, Convert.ToDateTime(value).Ticks);
                basic.Emit(OpCodes.Newobj, typeof(DateTime).GetConstructor(new Type[] { typeof(Int64) }));
            }
            else
            {
                throw new Exception("not exist datatype!");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void EmitValue(this EmitBasic basic, Object value, Type type)
        {
            if (type == typeof(String))
            {
                basic.Emit(OpCodes.Ldstr, Convert.ToString(value));
            }
            else if (type == typeof(Boolean))
            {
                switch (Convert.ToBoolean(value))
                {
                    case true: basic.Emit(OpCodes.Ldc_I4_1); break;
                    case false: basic.Emit(OpCodes.Ldc_I4_0); break;
                    default: throw new Exception("boolean to error!");
                }
            }
            else if (type == typeof(SByte))
            {
                basic.IntegerMap(Convert.ToSByte(value));
            }
            else if (type == typeof(Byte))
            {
                basic.IntegerMap((SByte)Convert.ToByte(value));
            }
            else if (type == typeof(Int16))
            {
                basic.IntegerMap(Convert.ToInt16(value));
            }
            else if (type == typeof(UInt16))
            {
                basic.IntegerMap((Int16)Convert.ToUInt16(value));
            }
            else if (type == typeof(Int32))
            {
                basic.IntegerMap(Convert.ToInt32(value));
            }
            else if (type == typeof(UInt32))
            {
                basic.IntegerMap((Int32)Convert.ToUInt32(value));
            }
            else if (type == typeof(Int64))
            {
                basic.IntegerMap(Convert.ToInt64(value));
            }
            else if (type == typeof(UInt64))
            {
                basic.IntegerMap((Int64)Convert.ToUInt64(value));
            }
            else if (type == typeof(Single))
            {
                basic.Emit(OpCodes.Ldc_R4, Convert.ToSingle(value));
            }
            else if (type == typeof(Double))
            {
                basic.Emit(OpCodes.Ldc_R8, Convert.ToDouble(value));
            }
            else if (type == typeof(Decimal))
            {
                Int32[] bits = Decimal.GetBits(Convert.ToDecimal(value));
                basic.IntegerMap(bits[0]);
                basic.IntegerMap(bits[1]);
                basic.IntegerMap(bits[2]);
                basic.Emit((bits[3] & 0x80000000) != 0 ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                basic.IntegerMap((bits[3] >> 16) & 0x7f);
                basic.Emit(OpCodes.Newobj, typeof(Decimal)
                    .GetConstructor(new Type[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Boolean), typeof(Byte) }));
            }
            else if (type == typeof(DateTime))
            {
                basic.Emit(OpCodes.Ldc_I8, Convert.ToDateTime(value).Ticks);
                basic.Emit(OpCodes.Newobj, typeof(DateTime).GetConstructor(new Type[] { typeof(Int64) }));
            }
            else
            {
                throw new Exception("not exist datatype!");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IntegerMap(this EmitBasic basic, Int64 value)
        {
            switch (value)
            {
                case -1: basic.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: basic.Emit(OpCodes.Ldc_I4_0); break;
                case 1: basic.Emit(OpCodes.Ldc_I4_1); break;
                case 2: basic.Emit(OpCodes.Ldc_I4_2); break;
                case 3: basic.Emit(OpCodes.Ldc_I4_3); break;
                case 4: basic.Emit(OpCodes.Ldc_I4_4); break;
                case 5: basic.Emit(OpCodes.Ldc_I4_5); break;
                case 6: basic.Emit(OpCodes.Ldc_I4_6); break;
                case 7: basic.Emit(OpCodes.Ldc_I4_7); break;
                case 8: basic.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (value < Int64.MinValue || value > Int64.MaxValue)
                    {
                        ShowEx("IntegerMap 数值溢出");
                    }
                    else if (value < Int32.MinValue || value > Int32.MaxValue)
                    {
                        basic.Emit(OpCodes.Ldc_I8, value);
                    }
                    else if (value < SByte.MinValue || value > SByte.MaxValue)
                    {
                        basic.Emit(OpCodes.Ldc_I4, value);
                    }
                    else
                    {
                        basic.Emit(OpCodes.Ldc_I4_S, value);
                    }
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void PopArray(this EmitBasic basic, Type type)
        {
            if (type == typeof(String))
            {
                basic.Emit(OpCodes.Ldelem_Ref);
            }
            else if (type == typeof(Boolean))
            {
                basic.Emit(OpCodes.Ldelem_I1);
            }
            else if (type == typeof(SByte))
            {
                basic.Emit(OpCodes.Ldelem_I1);
            }
            else if (type == typeof(Byte))
            {
                basic.Emit(OpCodes.Ldelem_I1);
            }
            else if (type == typeof(Int16))
            {
                basic.Emit(OpCodes.Ldelem_I2);
            }
            else if (type == typeof(UInt16))
            {
                basic.Emit(OpCodes.Ldelem_I2);
            }
            else if (type == typeof(Int32))
            {
                basic.Emit(OpCodes.Ldelem_I4);
            }
            else if (type == typeof(UInt32))
            {
                basic.Emit(OpCodes.Ldelem_I4);
            }
            else if (type == typeof(Int64))
            {
                basic.Emit(OpCodes.Ldelem_I8);
            }
            else if (type == typeof(UInt64))
            {
                basic.Emit(OpCodes.Ldelem_I8);
            }
            else if (type == typeof(Single))
            {
                basic.Emit(OpCodes.Ldelem_R4);
            }
            else if (type == typeof(Double))
            {
                basic.Emit(OpCodes.Ldelem_R8);
            }
            else if (type == typeof(Decimal))
            {
                basic.Emit(OpCodes.Ldelem);
            }
            else
            {
                throw new Exception("not exist datatype!");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void PushArray(this EmitBasic basic, Type type)
        {
            if (type == typeof(String))
            {
                basic.Emit(OpCodes.Stelem_Ref);
            }
            else if (type == typeof(Boolean))
            {
                basic.Emit(OpCodes.Stelem_I1);
            }
            else if (type == typeof(SByte))
            {
                basic.Emit(OpCodes.Stelem_I1);
            }
            else if (type == typeof(Byte))
            {
                basic.Emit(OpCodes.Stelem_I1);
            }
            else if (type == typeof(Int16))
            {
                basic.Emit(OpCodes.Stelem_I2);
            }
            else if (type == typeof(UInt16))
            {
                basic.Emit(OpCodes.Stelem_I2);
            }
            else if (type == typeof(Int32))
            {
                basic.Emit(OpCodes.Stelem_I4);
            }
            else if (type == typeof(UInt32))
            {
                basic.Emit(OpCodes.Stelem_I4);
            }
            else if (type == typeof(Int64))
            {
                basic.Emit(OpCodes.Stelem_I8);
            }
            else if (type == typeof(UInt64))
            {
                basic.Emit(OpCodes.Stelem_I8);
            }
            else if (type == typeof(Single))
            {
                basic.Emit(OpCodes.Stelem_R4);
            }
            else if (type == typeof(Double))
            {
                basic.Emit(OpCodes.Stelem_R8);
            }
            else if (type == typeof(Decimal))
            {
                basic.Emit(OpCodes.Stelem);
            }
            else
            {
                throw new Exception("not exist datatype!");
            }
        }
    }
}
