using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BigCookie.Document
{
    public class DocumentConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
        public void PostConfigure(string name, StaticFileOptions options)
        {
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), "BigCookie.Document.wwwroot");
            options.FileProvider = embeddedProvider;
        }
    }
}
