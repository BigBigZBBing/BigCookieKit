using Microsoft.Extensions.Logging;

using System;

namespace BigCookieKit.AspCore.Apm
{
    public class ApmEntityFrameworkLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly IServiceProvider _provider;

        public ApmEntityFrameworkLoggerProvider(Func<string, LogLevel, bool> filter, IServiceProvider provider)
        {
            _filter = filter;
            _provider = provider;
        }

        public ILogger CreateLogger(string categoryName) => new ApmEntityFrameworkLogger(categoryName, _filter, _provider);
        public void Dispose() { }
    }
}
