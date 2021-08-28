using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Network
{
    public class Pipeline
    {
        internal List<IPipe> pipes = new List<IPipe>();

        internal int _poistion;

        internal void Add(IPipe pipe)
        {
            pipes.Add(pipe);
        }

        internal void Start(Session session, Action callback)
        {
            _poistion = 0;
            if (pipes.Count == 0)
                callback?.Invoke();
            else Spin(session, callback);
        }

        internal void Spin(Session session, Action callback)
        {
            pipes[_poistion++].InvokeAsync(session,
                pipes.Count > _poistion
            ? delegate { Spin(session, callback); }
            : callback);
        }
    }
}
