using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigCookie.Document
{
    public class UriInfo
    {
        public string Uri { get; set; }

        public string Comment { get; set; }

        public string Method { get; set; }

    }

    public class BodyTable
    {
        public string FieldName { get; set; }

        public string TypeName { get; set; }

        public bool IsRequire { get; set; }

        public string Comment { get; set; }
    }
}
