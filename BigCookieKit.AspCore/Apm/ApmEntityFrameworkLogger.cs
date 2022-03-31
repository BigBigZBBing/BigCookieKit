using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BigCookieKit.AspCore.Apm
{
    public class ApmEntityFrameworkLogger : ILogger
    {
        private readonly string categoryName;
        private readonly Func<string, LogLevel, bool> filter;
        private readonly IServiceProvider provider;

        public ApmEntityFrameworkLogger(string categoryName, Func<string, LogLevel, bool> filter, IServiceProvider provider)
        {
            this.categoryName = categoryName;
            this.filter = filter;
            this.provider = provider;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel == LogLevel.Information
                && eventId.Name == "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted")
            {
                var accessor = provider.GetService<IHttpContextAccessor>();
                IReadOnlyList<KeyValuePair<string, object>> values = state as IReadOnlyList<KeyValuePair<string, object>>;

                if (accessor.HttpContext != null)
                {
                    var configuration = accessor.HttpContext.RequestServices.GetService<IConfiguration>();
                    switch (configuration.GetSection("ApmLogger:LoggerType")?.Value)
                    {
                        case "Console":
                            ApmLoggerCore.ConsoleEFLogger(accessor.HttpContext, new Dictionary<string, object?>(values), exception);
                            break;
                        case "SQLite":
                            ApmLoggerCore.SQLiteEFLogger(accessor.HttpContext, new Dictionary<string, object?>(values), exception);
                            break;
                    }
                }
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
