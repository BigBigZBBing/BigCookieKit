using BigCookieKit;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.Apm
{
    public class ApmLoggerFilter
    {
        private readonly RequestDelegate _next;

        public int Count { get; set; }

        public ApmLoggerFilter(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (string.IsNullOrEmpty(context.Request.Headers["TraceId"]))
            {
                context.Request.Headers.Add("TraceId", Snowflake.NextId().ToString());
            }
            string message = string.Empty;

            var orginstream = context.Response.Body;
            using var stream = new MemoryStream();
            context.Response.Body = stream;

            try
            {
                await _next?.Invoke(context);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            stopwatch.Stop();

            var configuration = context.RequestServices.GetService<IConfiguration>();
            switch (configuration.GetSection("ApmLogger:LoggerType")?.Value)
            {
                case "Console":
                    ApmLoggerCore.ConsoleApiLogger(context, stopwatch, message);
                    break;
                case "SQLite":
                    ApmLoggerCore.SQLiteApiLogger(context, stopwatch, message);
                    break;
            }

            await stream.CopyToAsync(orginstream);
            context.Response.Body = orginstream;
        }
    }
}
