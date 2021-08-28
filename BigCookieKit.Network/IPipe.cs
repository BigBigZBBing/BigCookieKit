using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Network
{
    public interface IPipe
    {
        Task InvokeAsync(Session session,Action context);
    }
}
