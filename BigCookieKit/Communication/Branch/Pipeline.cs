using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public class Pipeline
    {
        internal List<IPipe> pipes = new List<IPipe>();

        internal int _poistion;

        internal void Add(IPipe pipe)
        {
            pipes.Add(pipe);
        }

        internal void Start(Action callback)
        {
            _poistion = 0;
            if (pipes.Count == 0)
                callback?.Invoke();
            else Spin(callback);
        }

        internal void Spin(Action callback)
        {
            pipes[_poistion++].InvokeAsync(
                pipes.Count > _poistion
            ? delegate { Spin(callback); }
            : callback);
        }
    }
}
