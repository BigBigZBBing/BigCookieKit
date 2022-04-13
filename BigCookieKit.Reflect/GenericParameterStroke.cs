using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BigCookieKit.Reflect
{
    public sealed class GenericParameterStroke
    {
        internal List<Action<GenericTypeParameterBuilder>> builders = new List<Action<GenericTypeParameterBuilder>>();

        internal GenericParameterStroke() { }

        public GenericParameterStroke ConstraintClass<T>()
        {
            return ConstraintClass(typeof(T));
        }

        public GenericParameterStroke ConstraintClass(Type type)
        {
            builders.Add(x => x.SetBaseTypeConstraint(type));
            return this;
        }

        public GenericParameterStroke ConstraintInterface<T>()
        {
            return ConstraintInterface(typeof(T));
        }

        public GenericParameterStroke ConstraintInterface(params Type[] type)
        {
            builders.Add(x => x.SetInterfaceConstraints(type));
            return this;
        }

        public GenericParameterStroke ConstraintQualifier(GenericQualifier GenericType)
        {
            builders.Add(x => x.SetGenericParameterAttributes((GenericParameterAttributes)GenericType));
            return this;
        }

        internal void Builder(GenericTypeParameterBuilder genericTypeParameter)
        {
            builders.ForEach(x => x.Invoke(genericTypeParameter));
        }
    }
}
