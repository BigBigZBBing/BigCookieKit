using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BigCookieKit.Reflect
{
    public class SmartBuilder
    {
        private string dllName;
        private AssemblyName assmblyName;
        private AssemblyBuilder assemblyBuilder;
        private ModuleBuilder moduleBuilder;
        private TypeBuilder typeBuilder;
        private ConstructorBuilder constructorBuilder;
        private ConstructorBuilder publicBuilder;
        private ConstructorBuilder staticBuilder;
        private FieldBuilder fieldBuilder;
        private PropertyBuilder propertyBuilder;
        private MethodBuilder methodBuilder;
        private ILGenerator MainIL;
        private Type _dymaticType;
        private object _instance;

        public object Instance { get => _instance; set => _instance = value; }

        public SmartBuilder(string dllName)
        {
            this.dllName = dllName;
            ModuleProcess();
        }

        public SmartBuilder()
        {
            this.dllName = "BigCookie_" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
            ModuleProcess();
        }

        public void ModuleProcess()
        {
            assmblyName = new AssemblyName(this.dllName);

#if NET452
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assmblyName, AssemblyBuilderAccess.RunAndSave);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assmblyName.Name, $"{assmblyName.Name}.dll");
#else
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assmblyName, AssemblyBuilderAccess.RunAndCollect);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assmblyName.Name);
#endif
        }

        public ClassStroke Class(string ClassName, ClassQualifier ClassType = ClassQualifier.Public)
        {
            return Class(ClassName, (TypeAttributes)ClassType);
        }

        public ClassStroke Class(string ClassName, TypeAttributes ClassType)
        {
            typeBuilder = moduleBuilder.DefineType(ClassName, ClassType);

            publicBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);

            staticBuilder = typeBuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, Type.EmptyTypes);

            return new ClassStroke(typeBuilder, publicBuilder, staticBuilder);
        }

        public CtorStroke Ctor(Type[] ParamTypes, MethodQualifier MethodType = MethodQualifier.Public)
        {
            return Ctor(ParamTypes, (MethodAttributes)MethodType);
        }

        public CtorStroke Ctor(Type[] ParamTypes, MethodAttributes Attr)
        {
            if (ParamTypes.Equals(Type.EmptyTypes)) throw new ArrayTypeMismatchException();

            constructorBuilder = typeBuilder.DefineConstructor(Attr, CallingConventions.Standard, ParamTypes);

            if (Attr.HasFlag(MethodAttributes.Public))
            {
                constructorBuilder.GetILGenerator().Emit(OpCodes.Ldarg_0);
                constructorBuilder.GetILGenerator().Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            }

            return new CtorStroke(constructorBuilder);
        }

        public FieldStroke Field(string FieldName, Type Type, FieldQualifier FieldType = FieldQualifier.Public)
        {
            return Field(FieldName, Type, (FieldAttributes)FieldType);
        }

        public FieldStroke Field(string FieldName, Type Type, FieldAttributes Attr)
        {
            fieldBuilder = typeBuilder.DefineField(FieldName, Type, Attr);

            return new FieldStroke(fieldBuilder, publicBuilder, staticBuilder);
        }

        public PropertyStroke Property(string PropertyName, Type Type, PropertyQualifier PropertyType = PropertyQualifier.Public)
        {
            return Property(PropertyName, Type, (MethodAttributes)PropertyType);
        }

        public PropertyStroke Property(string PropertyName, Type Type, MethodAttributes Attr)
        {
            var fieldStroke = Field($"<{PropertyName}>k__BackingField", Type);
            fieldStroke.AddAttribute(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes), 01, 00, 00, 00);
            fieldStroke.AddAttribute(
                typeof(System.Diagnostics.DebuggerBrowsableAttribute).GetConstructor(new Type[] { typeof(System.Diagnostics.DebuggerBrowsableState) }),
                01, 00, 00, 00, 00, 00, 00, 00);
            propertyBuilder = typeBuilder.DefineProperty(PropertyName, PropertyAttributes.None, Type, null);
            get_Item(PropertyName, Type, Attr | MethodAttributes.HideBySig | MethodAttributes.SpecialName);
            set_Item(PropertyName, Type, Attr | MethodAttributes.HideBySig | MethodAttributes.SpecialName);
            return new PropertyStroke(propertyBuilder, fieldStroke);
        }

        public MethodStroke Method(string MethodName, Type RetType = null, Type[] ParamTypes = null, MethodQualifier MethodType = MethodQualifier.Public)
        {
            return Method(MethodName, RetType, ParamTypes, (MethodAttributes)MethodType);
        }

        public MethodStroke Method(string MethodName, Type RetType, Type[] ParamTypes, MethodAttributes Attr)
        {
            methodBuilder = typeBuilder.DefineMethod(
                MethodName,
                Attr,
                CallingConventions.Standard,
                RetType, null, null,
                ParamTypes, null, null);

            return new MethodStroke(methodBuilder);
        }

        internal void get_Item(string PropertyName, Type Type, MethodAttributes Attr)
        {
            methodBuilder = typeBuilder.DefineMethod(
                "get_" + PropertyName,
                Attr,
                CallingConventions.Standard,
                Type, null, null,
                null, null, null);
            methodBuilder.SetCustomAttribute(
                typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes),
                new byte[] { 01, 00, 00, 00 });
            MainIL = methodBuilder.GetILGenerator();
            if (Attr.HasFlag(MethodAttributes.Static))
            {
                MainIL.Emit(OpCodes.Ldsfld, fieldBuilder);
            }
            else
            {
                MainIL.Emit(OpCodes.Ldarg_0);
                MainIL.Emit(OpCodes.Ldfld, fieldBuilder);
            }
            MainIL.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(methodBuilder);
        }

        internal void set_Item(string PropertyName, Type Type, MethodAttributes Attr)
        {
            methodBuilder = typeBuilder.DefineMethod(
                "set_" + PropertyName,
                Attr,
                CallingConventions.Standard,
                null, null, null,
                new Type[] { Type }, null, null);
            methodBuilder.SetCustomAttribute(
                typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes),
                new byte[] { 01, 00, 00, 00 });
            MainIL = methodBuilder.GetILGenerator();

            if (Attr.HasFlag(MethodAttributes.Static))
            {
                MainIL.Emit(OpCodes.Ldarg_0);
                MainIL.Emit(OpCodes.Stsfld, fieldBuilder);
            }
            else
            {
                MainIL.Emit(OpCodes.Ldarg_0);
                MainIL.Emit(OpCodes.Ldarg_1);
                MainIL.Emit(OpCodes.Stfld, fieldBuilder);
            }
            MainIL.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(methodBuilder);
        }

        internal void SaveType()
        {
            publicBuilder.GetILGenerator().Emit(OpCodes.Ldarg_0);
            publicBuilder.GetILGenerator().Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            publicBuilder.GetILGenerator().Emit(OpCodes.Ret);
            staticBuilder.GetILGenerator().Emit(OpCodes.Ret);

            _dymaticType = typeBuilder.CreateTypeInfo();
        }

#if NET452

        public void Save()
        {
            SaveType();
            assemblyBuilder.Save($"{assmblyName.Name}.dll");
        }

#endif

        public object Generation(params object[] args)
        {
            SaveType();
            return _instance = Activator.CreateInstance(_dymaticType, args);
        }

        public FastDynamic InitEntity()
        {
            return FastDynamic.GetFastDynamic(Generation());
        }

        public static T DynamicMethod<T>(string MethodName, Action<FuncGenerator> builder) where T : class
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

            DynamicMethod dynamicBuilder = new DynamicMethod(MethodName, retType, types.ToArray());
            builder?.Invoke(new FuncGenerator(dynamicBuilder.GetILGenerator()));
            T deleg = dynamicBuilder.CreateDelegate(typeof(T)) as T;
            return deleg;
        }
    }
}