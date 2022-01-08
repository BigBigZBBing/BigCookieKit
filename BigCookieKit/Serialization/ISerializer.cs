using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.Serialization
{
    public class ISerializer
    {
        public Stream stream { get; set; }

        public void WriteLength(int length)
        {

        }
    }
}
