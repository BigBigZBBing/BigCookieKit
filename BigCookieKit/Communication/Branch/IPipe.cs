using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Communication
{
    public interface IPipe
    {
        Task InvokeAsync(Action context);
    }
}
