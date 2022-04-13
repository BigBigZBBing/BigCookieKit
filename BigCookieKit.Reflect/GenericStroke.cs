using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Reflect
{
    public sealed class GenericStroke
    {
        internal TypeBuilder typeBuilder;

        internal MethodBuilder methodBuilder;

        internal Dictionary<string, GenericParameterStroke> parameterNames = new Dictionary<string, GenericParameterStroke>();

        internal GenericStroke() { }

        internal GenericStroke(TypeBuilder typeBuilder)
        {
            this.typeBuilder = typeBuilder;
        }

        internal GenericStroke(MethodBuilder methodBuilder)
        {
            this.methodBuilder = methodBuilder;
        }

        public GenericParameterStroke AddGenericParameter(string name)
        {
            var stroke = new GenericParameterStroke();
            parameterNames.Add(name, stroke);
            return stroke;
        }

        internal void Builder()
        {
            GenericTypeParameterBuilder[] builders = null;
            builders ??= typeBuilder?.DefineGenericParameters(parameterNames.Select(x => x.Key).ToArray());
            builders ??= methodBuilder?.DefineGenericParameters(parameterNames.Select(x => x.Key).ToArray());
            foreach (var item in builders)
                parameterNames[item.Name].Builder(item);
        }
    }
}
