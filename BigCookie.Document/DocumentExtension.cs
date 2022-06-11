using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

using System;
using System.Reflection;

namespace BigCookie.Document
{
    public static class DocumentExtension
    {
        internal static IApplicationBuilder _application;

        public static void UseDocumentUI(this IApplicationBuilder app)
        {
            _application = app;

            var options = new StaticFileOptions();
            {
                var embeddedProvider = new ManifestEmbeddedFileProvider(Assembly.GetExecutingAssembly(), "wwwroot");
                options.FileProvider = embeddedProvider;
            }
            app.UseStaticFiles(options);
        }
    }
}
