using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public class FastProperty
    {
        private PropertySetterEmit setter;

        private PropertyGetterEmit getter;

        public string PropertyName { get; private set; }

        public Type PropertyType { get; private set; }

        public object Instance { get; private set; }

        public MethodInfo GetMethod { get; private set; }

        public MethodInfo SetMethod { get; private set; }

        public FastProperty(PropertyInfo propertyInfo, object Instance = null)
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

        public void Set(object value)
        {
            if (Instance == null)
            {
                throw new ArgumentNullException("实例为空");
            }
            this.setter?.Invoke(Instance, value);
        }

        public object Get()
        {
            if (Instance == null)
            {
                throw new ArgumentNullException("实例为空");
            }
            return this.getter?.Invoke(Instance);
        }

        public void Set(object instance, object value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("实例为空");
            }
            this.setter?.Invoke(instance, value);
        }

        public object Get(object instance)
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
        private readonly Func<object, object> getter;

        public PropertyGetterEmit(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            this.getter = CreateGetterEmit(propertyInfo);
        }

        public object Invoke(object instance)
        {
            return getter?.Invoke(instance);
        }

        private Func<object, object> CreateGetterEmit(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            MethodInfo getMethod = property.GetGetMethod(true);

            DynamicMethod dm = new DynamicMethod("PropertyGetter", typeof(object), new Type[] { typeof(object) }, property.DeclaringType, true);

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

            return (Func<object, object>)dm.CreateDelegate(typeof(Func<object, object>));
        }
    }

    internal class PropertySetterEmit
    {
        private readonly Action<object, object> setFunc;

        public PropertySetterEmit(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            this.setFunc = CreatePropertySetter(propertyInfo);
        }

        private Action<object, object> CreatePropertySetter(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            MethodInfo setMethod = property.GetSetMethod(true);

            DynamicMethod dm = new DynamicMethod("PropertySetter", null, new Type[] { typeof(object), typeof(object) }, property.DeclaringType, true);

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

            return (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
        }

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

        public void Invoke(object instance, object value)
        {
            this.setFunc?.Invoke(instance, value);
        }
    }
}