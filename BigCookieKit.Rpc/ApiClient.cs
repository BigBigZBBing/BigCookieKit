using BigCookieKit.Network;
using BigCookieKit.Reflect;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;

namespace BigCookieKit.Rpc
{
    public class RpcClient
    {
        NetworkClient client { get; set; }

        BlockingCollection<object> blocking = new BlockingCollection<object>();

        public RpcClient(string host, int port)
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

            var _client = builder.Property("_client", typeof(RpcClient));

            var ctorStorke = builder.Ctor(new Type[] { typeof(RpcClient) });
            ctorStorke.Builder(il =>
            {
                _client.SetValue(il, il.ArgumentRef<RpcClient>(1));
            }).End();

            foreach (var method in methods)
            {
                var methodStorke = builder.Method(
                      method.Name,
                      method.ReturnType,
                      method.GetParameters().Select(x => x.ParameterType).ToArray(),
                      MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);

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
                    var retCall = client.Call("TransmitContact", il.String($"{type.FullName}.{method.Name}"), dicObj);
                    if (method.ReturnType != typeof(void))
                        il.Object(retCall.ReturnRef()).As(method.ReturnType).Output();
                }).End();
            }

            return builder.Generation(this) as T;
        }

        private object TransmitContact(string router, Dictionary<string, object> packet)
        {
            client.Start();
            client.OnConnect = (session) =>
            {
                //session.SendMessage()
            };
            blocking.TryTake(out object obj);
            client.Close();
            return obj;
        }
    }
}
