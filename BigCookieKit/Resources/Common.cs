using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BigCookieKit.Resources
{
    public static class Common
    {
        public static Stream GetXlsxResource(string fileName)
        {
            Assembly Asmb = Assembly.GetExecutingAssembly();
            return Asmb.GetManifestResourceStream("BigCookieKit.Resources.xlsx." + fileName);
        }

        public static Stream GetXlsxResourceString(string fileName)
        {
            Assembly Asmb = Assembly.GetExecutingAssembly();
            return Asmb.GetManifestResourceStream("BigCookieKit.Resources.xlsx." + fileName);
        }
    }
}
