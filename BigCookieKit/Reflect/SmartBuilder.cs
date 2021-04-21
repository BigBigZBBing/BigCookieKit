using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BigCookieKit.Reflect
{
    public class SmartBuilder
    {
        private String dllName;
        private AssemblyName assmblyName;
        private AssemblyBuilder assemblyBuilder;
        private ModuleBuilder moduleBuilder;
        private TypeBuilder typeBuilder;
        private ConstructorBuilder constructorBuilder;
        private FieldBuilder fieldBuilder;
        private PropertyBuilder propertyBuilder;
        private MethodBuilder methodBuilder;
        private ILGenerator MainIL;
        private Type _dymaticType;
        private Object _instance;

        private static readonly Object _lock = new Object();

        public object Instance { get => _instance; set => _instance = value; }

        public SmartBuilder(String dllName)
        {
            this.dllName = dllName;
        }

        public SmartBuilder Assembly()
        {
            assmblyName = new AssemblyName(dllName);

#if NET452
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assmblyName, AssemblyBuilderAccess.RunAndSave);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assmblyName.Name, $"{assmblyName.Name}.dll");
#else
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assmblyName, AssemblyBuilderAccess.RunAndCollect);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assmblyName.Name);
#endif

            return this;
        }

        public ClassIL Class(String ClassName, Qualifier ClassType = Qualifier.Public)
        {
            typeBuilder = moduleBuilder.DefineType(ClassName, (TypeAttributes)ClassType);

            return new ClassIL(typeBuilder);
        }

        public CtorIL Ctor(Type[] ParamTypes, Action<FuncGenerator> builder, MethodAttributes Attr = MethodAttributes.Public)
        {
            constructorBuilder = typeBuilder.DefineConstructor(Attr, CallingConventions.Standard, ParamTypes);

            builder?.Invoke(new FuncGenerator(constructorBuilder.GetILGenerator()));

            return new CtorIL(constructorBuilder);
        }

        public FieldIL Field(String FieldName, Type Type, FieldAttributes Attr = FieldAttributes.Private, Object ConstValue = null)
        {
            fieldBuilder = typeBuilder.DefineField(FieldName, Type, Attr);

            if (ConstValue != null) fieldBuilder.SetConstant(ConstValue);

            return new FieldIL(fieldBuilder);
        }

        public PropertyIL Property(String PropertyName, Type Type, PropertyAttributes Attr = PropertyAttributes.None)
        {
            propertyBuilder = typeBuilder.DefineProperty(PropertyName, Attr, Type, null);

            return new PropertyIL(propertyBuilder);
        }

        public MethodIL Method(String MethodName, Action<FuncGenerator> builder, Type RetType = null, Type[] ParamTypes = null, MethodAttributes Attr = MethodAttributes.Public)
        {
            methodBuilder = typeBuilder.DefineMethod(MethodName, Attr, RetType, ParamTypes);

            builder?.Invoke(new FuncGenerator(methodBuilder.GetILGenerator()));

            return new MethodIL(methodBuilder);
        }

        public void get_Item(Type Type)
        {
            Method("get_Item", null, Type, Type.EmptyTypes, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig);

            MainIL = methodBuilder.GetILGenerator();
            MainIL.Emit(OpCodes.Ldarg_0);
            MainIL.Emit(OpCodes.Ldfld, fieldBuilder);
            MainIL.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(methodBuilder);
        }

        public void set_Item(Type Type)
        {
            Method("set_Item", null, null, new Type[] { Type }, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig);

            MainIL = methodBuilder.GetILGenerator();
            MainIL.Emit(OpCodes.Ldarg_0);
            MainIL.Emit(OpCodes.Ldarg_1);
            MainIL.Emit(OpCodes.Stfld, fieldBuilder);
            MainIL.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(methodBuilder);
        }

        public void SaveClass()
        {
            _dymaticType = typeBuilder.CreateTypeInfo();
        }

#if NET452
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Save()
        {
            SaveClass();
            assemblyBuilder.Save($"{assmblyName.Name}.dll");
        }
#endif

        public object Build()
        {
            return _instance = Activator.CreateInstance(_dymaticType);
        }

        public void CreateProperty(String FieldName, Type FieldType)
        {
            Field($"_{FieldName}", FieldType);
            Property(FieldName, FieldType);
            get_Item(FieldType);
            set_Item(FieldType);
        }

        public FastDynamic InitEntity()
        {
            SaveClass();
            return FastDynamic.GetFastDynamic(Build());
        }

        public static T DynamicMethod<T>(String MethodName, Action<FuncGenerator> builder) where T : class
        {
            var type = typeof(T);

            if (!type.Name.StartsWith("Func`") && !type.Name.StartsWith("Action"))
                throw new Exception("please use Func or Action");

            var types = type.GenericTypeArguments.ToList();

            Type retType = null;
            if (type.Name.StartsWith("Func`") && types != null && types.Count > 0)
            {
                retType = types.Last();
                types.RemoveAt(types.Count - 1);
            }

            Monitor.Enter(_lock);

            DynamicMethod dynamicBuilder = new DynamicMethod(MethodName, retType, types.ToArray());

            builder?.Invoke(new FuncGenerator(dynamicBuilder.GetILGenerator()));

            T deleg = dynamicBuilder.CreateDelegate(typeof(T)) as T;

            Monitor.Exit(_lock);

            return deleg;
        }
    }
}
