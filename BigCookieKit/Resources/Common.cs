using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BigCookieKit.Resources
{
    public static class Common
    {
        private const string ResourcePath = "BigCookieKit.Resources.";

        public static Stream GetXlsxResource(string fileName)
        {
            Assembly Asmb = Assembly.GetExecutingAssembly();
            return Asmb.GetManifestResourceStream(ResourcePath + "xlsx." + fileName);
        }

        public static string GetXlsxResourceString(string fileName)
        {
            Assembly Asmb = Assembly.GetExecutingAssembly();
            using Stream stream = Asmb.GetManifestResourceStream(ResourcePath + "xlsx." + fileName);
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
    }
}
