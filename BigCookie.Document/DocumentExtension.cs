using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

using System;
using System.Reflection;

namespace BigCookie.Document
{
    public static class DocumentExtension
    {
        internal static IApplicationBuilder _application;
        internal static string _description = "";

        public static void UseDocumentUI(this IApplicationBuilder app, string description = "")
        {
            _application = app;
            _description = description;

            var options = new StaticFileOptions();
            {
                var embeddedProvider = new ManifestEmbeddedFileProvider(Assembly.GetExecutingAssembly(), "wwwroot");
                options.FileProvider = embeddedProvider;
            }
            app.UseStaticFiles(options);
        }
    }
}
