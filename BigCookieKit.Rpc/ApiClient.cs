using BigCookieKit.Network;
using BigCookieKit.Reflect;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;

namespace BigCookieKit.Rpc
{
    public class ApiClient
    {
        NetworkClient client { get; set; }

        public ApiClient(string host, int port)
        {
            client = new NetworkClient(host, port);
        }

        public T GetInstance<T>()
            where T : class
        {
            var type = typeof(T);

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            SmartBuilder builder = new SmartBuilder();
            var classStorke = builder.Class(type.Name + ".Instance");
            classStorke.InheritInterface(type);

            var _client = builder.Property("_client", typeof(ApiClient));

            var ctorStorke = builder.Ctor(new Type[] { typeof(ApiClient) });
            ctorStorke.Builder(il =>
            {
                _client.SetValue(il, il.ArgumentRef<ApiClient>(1));
            }).End();

            foreach (var method in methods)
            {
                var methodStorke = builder.Method(
                      method.Name,
                      method.ReturnType,
                      method.GetParameters().Select(x => x.ParameterType).ToArray(),
                      MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);

                methodStorke.Builder(il =>
                {
                    var dic = il.Initialize(typeof(Dictionary<string, object>));
                    var dicObj = il.Object(dic);

                    int paramIndex = 1;
                    foreach (var item in method.GetParameters())
                    {
                        var arg = il.ArgumentRef(paramIndex, item.ParameterType);
                        dicObj.Call("Add", il.String("param_" + paramIndex), arg);
                        paramIndex++;
                    }

                    var client = il.Object(_client.GetValue(il));
                    client.Call("TransmitContact", il.String($"{type.FullName}.{method.Name}"), dicObj);

                }).End();
            }

            return builder.Generation(this) as T;
        }

        private void TransmitContact(string router, Dictionary<string, object> packet)
        {

        }
    }
}
