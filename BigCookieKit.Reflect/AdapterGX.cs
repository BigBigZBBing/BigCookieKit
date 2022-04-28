using System;
using System.IO;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    internal static partial class ManagerGX
    {
        internal static void EmitValue<T>(this EmitBasic basic, T value)
        {
            if (value == null)
            {
                basic.Emit(OpCodes.Ldnull);
            }
            else if (value.GetType() == typeof(string))
            {
                basic.Emit(OpCodes.Ldstr, Convert.ToString(value));
            }
            else if (value.GetType() == typeof(bool))
            {
                switch (Convert.ToBoolean(value))
                {
                    case true: basic.Emit(OpCodes.Ldc_I4_1); break;
                    case false: basic.Emit(OpCodes.Ldc_I4_0); break;
                    default: throw new Exception("boolean to error!");
                }
            }
            else if (value.GetType() == typeof(sbyte))
            {
                basic.IntegerMap(Convert.ToSByte(value));
            }
            else if (value.GetType() == typeof(byte))
            {
                basic.IntegerMap((sbyte)Convert.ToByte(value));
            }
            else if (value.GetType() == typeof(short))
            {
                basic.IntegerMap(Convert.ToInt16(value));
            }
            else if (value.GetType() == typeof(ushort))
            {
                basic.IntegerMap((short)Convert.ToUInt16(value));
            }
            else if (value.GetType() == typeof(int))
            {
                basic.IntegerMap(Convert.ToInt32(value));
            }
            else if (value.GetType() == typeof(uint))
            {
                basic.IntegerMap((int)Convert.ToUInt32(value));
            }
            else if (value.GetType() == typeof(long))
            {
                basic.IntegerMap(Convert.ToInt64(value));
            }
            else if (value.GetType() == typeof(ulong))
            {
                basic.IntegerMap((long)Convert.ToUInt64(value));
            }
            else if (value.GetType() == typeof(float))
            {
                basic.Emit(OpCodes.Ldc_R4, Convert.ToSingle(value));
            }
            else if (value.GetType() == typeof(double))
            {
                basic.Emit(OpCodes.Ldc_R8, Convert.ToDouble(value));
            }
            else if (value.GetType() == typeof(decimal))
            {
                int[] bits = decimal.GetBits(Convert.ToDecimal(value));
                basic.IntegerMap(bits[0]);
                basic.IntegerMap(bits[1]);
                basic.IntegerMap(bits[2]);
                basic.Emit((bits[3] & 0x80000000) != 0 ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                basic.IntegerMap((bits[3] >> 16) & 0x7f);
                basic.Emit(OpCodes.Newobj, typeof(decimal)
                    .GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) }));
            }
            else if (value.GetType() == typeof(DateTime))
            {
                basic.Emit(OpCodes.Ldc_I8, Convert.ToDateTime(value).Ticks);
                basic.Emit(OpCodes.Newobj, typeof(DateTime).GetConstructor(new Type[] { typeof(long) }));
            }
            else if (value is Enum)
            {
                basic.IntegerMap(Convert.ToInt32(value));
            }
            else if (value.GetType().IsClass)
            {
                basic.Emit(OpCodes.Newobj, value.GetType().GetConstructor(Type.EmptyTypes));
            }
            else
            {
                throw new Exception("not exist datatype!");
            }
        }

        internal static void EmitValue(this EmitBasic basic, object value, Type type)
        {
            if (type == typeof(string))
            {
                basic.Emit(OpCodes.Ldstr, Convert.ToString(value));
            }
            else if (type == typeof(bool))
            {
                switch (Convert.ToBoolean(value))
                {
                    case true: basic.Emit(OpCodes.Ldc_I4_1); break;
                    case false: basic.Emit(OpCodes.Ldc_I4_0); break;
                    default: throw new Exception("boolean to error!");
                }
            }
            else if (type == typeof(sbyte))
            {
                basic.IntegerMap(Convert.ToSByte(value));
            }
            else if (type == typeof(byte))
            {
                basic.IntegerMap((sbyte)Convert.ToByte(value));
            }
            else if (type == typeof(short))
            {
                basic.IntegerMap(Convert.ToInt16(value));
            }
            else if (type == typeof(ushort))
            {
                basic.IntegerMap((short)Convert.ToUInt16(value));
            }
            else if (type == typeof(int))
            {
                basic.IntegerMap(Convert.ToInt32(value));
            }
            else if (type == typeof(uint))
            {
                basic.IntegerMap((int)Convert.ToUInt32(value));
            }
            else if (type == typeof(long))
            {
                basic.IntegerMap(Convert.ToInt64(value));
            }
            else if (type == typeof(ulong))
            {
                basic.IntegerMap((long)Convert.ToUInt64(value));
            }
            else if (type == typeof(float))
            {
                basic.Emit(OpCodes.Ldc_R4, Convert.ToSingle(value));
            }
            else if (type == typeof(double))
            {
                basic.Emit(OpCodes.Ldc_R8, Convert.ToDouble(value));
            }
            else if (type == typeof(decimal))
            {
                int[] bits = decimal.GetBits(Convert.ToDecimal(value));
                basic.IntegerMap(bits[0]);
                basic.IntegerMap(bits[1]);
                basic.IntegerMap(bits[2]);
                basic.Emit((bits[3] & 0x80000000) != 0 ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                basic.IntegerMap((bits[3] >> 16) & 0x7f);
                basic.Emit(OpCodes.Newobj, typeof(decimal)
                    .GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) }));
            }
            else if (type == typeof(DateTime))
            {
                basic.Emit(OpCodes.Ldc_I8, Convert.ToDateTime(value).Ticks);
                basic.Emit(OpCodes.Newobj, typeof(DateTime).GetConstructor(new Type[] { typeof(long) }));
            }
            else
            {
                throw new Exception("not exist datatype!");
            }
        }

        internal static void IntegerMap(this EmitBasic basic, long value)
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
                    if (value < long.MinValue || value > long.MaxValue)
                    {
                        ShowEx("IntegerMap 数值溢出");
                    }
                    else if (value < int.MinValue || value > int.MaxValue)
                    {
                        basic.Emit(OpCodes.Ldc_I8, value);
                    }
                    else if (value < sbyte.MinValue || value > sbyte.MaxValue)
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

        internal static void PopArray(this EmitBasic basic, Type type)
        {
            if (type == typeof(string))
            {
                basic.Emit(OpCodes.Ldelem_Ref);
            }
            else if (type == typeof(bool))
            {
                basic.Emit(OpCodes.Ldelem_I1);
            }
            else if (type == typeof(sbyte))
            {
                basic.Emit(OpCodes.Ldelem_I1);
            }
            else if (type == typeof(byte))
            {
                basic.Emit(OpCodes.Ldelem_I1);
            }
            else if (type == typeof(short))
            {
                basic.Emit(OpCodes.Ldelem_I2);
            }
            else if (type == typeof(ushort))
            {
                basic.Emit(OpCodes.Ldelem_I2);
            }
            else if (type == typeof(int))
            {
                basic.Emit(OpCodes.Ldelem_I4);
            }
            else if (type == typeof(uint))
            {
                basic.Emit(OpCodes.Ldelem_I4);
            }
            else if (type == typeof(long))
            {
                basic.Emit(OpCodes.Ldelem_I8);
            }
            else if (type == typeof(ulong))
            {
                basic.Emit(OpCodes.Ldelem_I8);
            }
            else if (type == typeof(float))
            {
                basic.Emit(OpCodes.Ldelem_R4);
            }
            else if (type == typeof(double))
            {
                basic.Emit(OpCodes.Ldelem_R8);
            }
            else if (type == typeof(decimal))
            {
                basic.Emit(OpCodes.Ldelem);
            }
            else
            {
                throw new Exception("not exist datatype!");
            }
        }

        internal static void PushArray(this EmitBasic basic, Type type)
        {
            if (type == typeof(string))
            {
                basic.Emit(OpCodes.Stelem_Ref);
            }
            else if (type == typeof(bool))
            {
                basic.Emit(OpCodes.Stelem_I1);
            }
            else if (type == typeof(sbyte))
            {
                basic.Emit(OpCodes.Stelem_I1);
            }
            else if (type == typeof(byte))
            {
                basic.Emit(OpCodes.Stelem_I1);
            }
            else if (type == typeof(short))
            {
                basic.Emit(OpCodes.Stelem_I2);
            }
            else if (type == typeof(ushort))
            {
                basic.Emit(OpCodes.Stelem_I2);
            }
            else if (type == typeof(int))
            {
                basic.Emit(OpCodes.Stelem_I4);
            }
            else if (type == typeof(uint))
            {
                basic.Emit(OpCodes.Stelem_I4);
            }
            else if (type == typeof(long))
            {
                basic.Emit(OpCodes.Stelem_I8);
            }
            else if (type == typeof(ulong))
            {
                basic.Emit(OpCodes.Stelem_I8);
            }
            else if (type == typeof(float))
            {
                basic.Emit(OpCodes.Stelem_R4);
            }
            else if (type == typeof(double))
            {
                basic.Emit(OpCodes.Stelem_R8);
            }
            else if (type == typeof(decimal))
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