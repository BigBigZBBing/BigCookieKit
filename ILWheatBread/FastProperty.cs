using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ILWheatBread
{
    public class FastProperty
    {
        private PropertySetterEmit setter;

        private PropertyGetterEmit getter;

        public String PropertyName { get; private set; }

        public Type PropertyType { get; private set; }

        public Object Instance { get; private set; }

        public MethodInfo GetMethod { get; private set; }

        public MethodInfo SetMethod { get; private set; }

        
        public FastProperty(PropertyInfo propertyInfo, Object Instance = null)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("属性不能为空");
            }

            if (propertyInfo.CanWrite)
            {
                setter = new PropertySetterEmit(propertyInfo);
            }

            if (propertyInfo.CanRead)
            {
                getter = new PropertyGetterEmit(propertyInfo);
            }

            this.PropertyName = propertyInfo.Name;

            this.PropertyType = propertyInfo.PropertyType;

            this.GetMethod = propertyInfo.GetGetMethod();

            this.SetMethod = propertyInfo.GetSetMethod();

            this.Instance = Instance;
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Object value)
        {
            if (Instance == null)
            {
                throw new ArgumentNullException("实例为空");
            }
            this.setter?.Invoke(Instance, value);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Object Get()
        {
            if (Instance == null)
            {
                throw new ArgumentNullException("实例为空");
            }
            return this.getter?.Invoke(Instance);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Object instance, Object value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("实例为空");
            }
            this.setter?.Invoke(instance, value);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Object Get(Object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("实例为空");
            }
            return this.getter?.Invoke(instance);
        }
    }

    internal class PropertyGetterEmit
    {
        private readonly Func<Object, Object> getter;

        
        public PropertyGetterEmit(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            this.getter = CreateGetterEmit(propertyInfo);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Object Invoke(Object instance)
        {
            return getter?.Invoke(instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Func<Object, Object> CreateGetterEmit(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            MethodInfo getMethod = property.GetGetMethod(true);

            DynamicMethod dm = new DynamicMethod("PropertyGetter", typeof(Object), new Type[] { typeof(Object) }, property.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!getMethod.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);

                il.EmitCall(OpCodes.Callvirt, getMethod, null);
            }
            else
            {
                il.EmitCall(OpCodes.Call, getMethod, null);
            }

            if (property.PropertyType.IsValueType)
                il.Emit(OpCodes.Box, property.PropertyType);

            il.Emit(OpCodes.Ret);

            return (Func<Object, Object>)dm.CreateDelegate(typeof(Func<Object, Object>));
        }
    }

    internal class PropertySetterEmit
    {
        private readonly Action<Object, Object> setFunc;

        public PropertySetterEmit(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            this.setFunc = CreatePropertySetter(propertyInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Action<Object, Object> CreatePropertySetter(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            MethodInfo setMethod = property.GetSetMethod(true);

            DynamicMethod dm = new DynamicMethod("PropertySetter", null, new Type[] { typeof(Object), typeof(Object) }, property.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!setMethod.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }

            il.Emit(OpCodes.Ldarg_1);

            EmitCastToReference(il, property.PropertyType);

            if (!setMethod.IsStatic && !property.DeclaringType.IsValueType)
            {
                il.EmitCall(OpCodes.Callvirt, setMethod, null);
            }
            else
            {
                il.EmitCall(OpCodes.Call, setMethod, null);
            }

            il.Emit(OpCodes.Ret);

            return (Action<Object, Object>)dm.CreateDelegate(typeof(Action<Object, Object>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(Object instance, Object value)
        {
            this.setFunc?.Invoke(instance, value);
        }
    }
}
