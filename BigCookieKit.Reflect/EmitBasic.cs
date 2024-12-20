﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public abstract class EmitBasic
    {
        internal ILGenerator generator;

        internal bool tiggerPop;

        private Dictionary<Type, Delegate> emitMethod => new Dictionary<Type, Delegate>();

        internal Type generatorType => typeof(ILGenerator);

        internal EmitBasic(ILGenerator generator)
        {
            this.generator = generator;
        }

        public static implicit operator ILGenerator(EmitBasic basic) => basic.generator;

        public void BeginCatchBlock(Type exceptionType) => generator.BeginCatchBlock(exceptionType);

        public void BeginExceptFilterBlock() => generator.BeginExceptFilterBlock();

        public Label BeginExceptionBlock() => generator.BeginExceptionBlock();

        public void BeginFaultBlock() => generator.BeginFaultBlock();

        public void BeginFinallyBlock() => generator.BeginFinallyBlock();

        public void BeginScope() => generator.BeginScope();

        public void Emit(OpCode opcode, string str) => DispatchEmit(opcode, str);

        public void Emit(OpCode opcode, FieldInfo field) => DispatchEmit(opcode, field);

        public void Emit(OpCode opcode, Label[] labels) => DispatchEmit(opcode, labels);

        public void Emit(OpCode opcode, Label label) => DispatchEmit(opcode, label);

        public void Emit(OpCode opcode, LocalBuilder local) => DispatchEmit(opcode, local);

        public void Emit(OpCode opcode, float arg) => DispatchEmit(opcode, arg);

        public void Emit(OpCode opcode, byte arg) => DispatchEmit(opcode, arg);

        public void Emit(OpCode opcode, sbyte arg) => DispatchEmit(opcode, arg);

        public void Emit(OpCode opcode, short arg) => DispatchEmit(opcode, arg);

        public void Emit(OpCode opcode, double arg) => DispatchEmit(opcode, arg);

        public void Emit(OpCode opcode, MethodInfo meth) => DispatchEmit(opcode, meth);

        public void Emit(OpCode opcode, int arg) => DispatchEmit(opcode, arg);

        public void Emit(OpCode opcode, long arg) => DispatchEmit(opcode, arg);

        public void Emit(OpCode opcode, Type cls) => DispatchEmit(opcode, cls);

        public void Emit(OpCode opcode, SignatureHelper signature) => DispatchEmit(opcode, signature);

        public void Emit(OpCode opcode, ConstructorInfo con) => DispatchEmit(opcode, con);

        public void Emit(OpCode opcode) => DispatchEmit(opcode);

        public void MarkLabel(Label loc) => generator.MarkLabel(loc);

        public LocalBuilder DeclareLocal(Type localType, bool pinned) => RedirectLocal(localType, pinned);

        public LocalBuilder DeclareLocal(Type localType) => RedirectLocal(localType);

        public Label DefineLabel() => generator.DefineLabel();

        public void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes) =>
            generator.EmitCall(opcode, methodInfo, optionalParameterTypes);

        public void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) =>
            generator.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);

        public void EmitWriteLine(string value) => generator.EmitWriteLine(value);

        public void EmitWriteLine(FieldInfo fld) => generator.EmitWriteLine(fld);

        public void EmitWriteLine(LocalBuilder localBuilder) => generator.EmitWriteLine(localBuilder);

        public void EndExceptionBlock() => generator.EndExceptionBlock();

        public void EndScope() => generator.EndScope();

        public void ThrowException(Type excType) => generator.ThrowException(excType);

        public void UsingNamespace(string usingNamespace) => generator.UsingNamespace(usingNamespace);

        private void DispatchEmit<T>(OpCode opcode, T value)
        {
            CheckOverLength(ref opcode);
            if (tiggerPop)
            {
                generator.Emit(OpCodes.Pop);
                tiggerPop = false;
            }
            ((Action<OpCode, T>)CacheMethod<T>()).Invoke(opcode, value);
        }

        private void DispatchEmit(OpCode opcode)
        {
            if (tiggerPop)
            {
                generator.Emit(OpCodes.Pop);
                tiggerPop = false;
            }
            generator.Emit(opcode);
        }

        private void CheckOverLength(ref OpCode opcode)
        {
#if NOTSHORTFORMAT
            if (generator.ILOffset > byte.MaxValue)
            {
#endif
            if (OpCodes.Stloc_S == opcode) opcode = OpCodes.Stloc;
            if (OpCodes.Ldloc_S == opcode) opcode = OpCodes.Ldloc;
            if (OpCodes.Ldloca_S == opcode) opcode = OpCodes.Ldloca;
            if (OpCodes.Beq_S == opcode) opcode = OpCodes.Beq;
            if (OpCodes.Bge_S == opcode) opcode = OpCodes.Bge;
            if (OpCodes.Bge_Un_S == opcode) opcode = OpCodes.Bge_Un;
            if (OpCodes.Bgt_S == opcode) opcode = OpCodes.Bgt;
            if (OpCodes.Bgt_Un_S == opcode) opcode = OpCodes.Bgt_Un;
            if (OpCodes.Ble_S == opcode) opcode = OpCodes.Ble;
            if (OpCodes.Ble_Un_S == opcode) opcode = OpCodes.Ble_Un;
            if (OpCodes.Blt_S == opcode) opcode = OpCodes.Blt;
            if (OpCodes.Blt_Un_S == opcode) opcode = OpCodes.Blt_Un;
            if (OpCodes.Bne_Un_S == opcode) opcode = OpCodes.Bne_Un;
            if (OpCodes.Br_S == opcode) opcode = OpCodes.Br;
            if (OpCodes.Brfalse_S == opcode) opcode = OpCodes.Brfalse;
            if (OpCodes.Brtrue_S == opcode) opcode = OpCodes.Brtrue;
            if (OpCodes.Ldarg_S == opcode) opcode = OpCodes.Ldarg;
            if (OpCodes.Ldarga_S == opcode) opcode = OpCodes.Ldarga;
            if (OpCodes.Ldc_I4_S == opcode) opcode = OpCodes.Ldc_I4;
            if (OpCodes.Leave_S == opcode) opcode = OpCodes.Leave;
            if (OpCodes.Starg_S == opcode) opcode = OpCodes.Starg;
#if NOTSHORTFORMAT
            }
#endif
        }

        private LocalBuilder RedirectLocal(Type localType, bool pinned = false)
        {
            var local = generator.DeclareLocal(localType, pinned);
            return local;
        }

        private Delegate CacheMethod<T>()
        {
            if (emitMethod.ContainsKey(typeof(T)))
            {
                return emitMethod[typeof(T)];
            }
            MethodInfo method = generatorType.GetMethod("Emit", new[] { typeof(OpCode), typeof(T) });
            Delegate deleg = method.CreateDelegate(typeof(Action<OpCode, T>), generator);
            emitMethod.Add(typeof(T), deleg);
            return deleg;
        }
    }
}