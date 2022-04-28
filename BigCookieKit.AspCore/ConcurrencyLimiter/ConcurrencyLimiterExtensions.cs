using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BigCookieKit.AspCore.ConcurrencyLimiter
{
    public static class ConcurrencyLimiterExtensions
    {
        /// <summary>
        /// 注入接口限流中间件
        /// <code>
        /// appsetting:
        /// 节点名:ConcurrencyLimiter
        /// 类型:数组
        /// 属性介绍:
        /// RequestPath:请求路由
        /// RequestQueueLimit:队列最大等待数量
        /// MaxConcurrentRequests:最大并行数量
        /// QueueFullMessage:队列满返回消息 默认:The queue is full!
        /// </code>
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConcurrencyLimiter(this IApplicationBuilder application)
        {
            return application.UseMiddleware(typeof(ConcurrencyLimiterMiddleware));
        }
    }

    public class ConcurrencyLimiterMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private static readonly ConcurrentDictionary<string, ActionLimter> _cacheQueue = new ConcurrentDictionary<string, ActionLimter>();

        public ConcurrencyLimiterMiddleware(RequestDelegate nextDelegate, IConfiguration configuration)
        {
            _nextDelegate = nextDelegate;
            _configuration = configuration;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var options = new List<ConcurrencyLimiterOption>();
            _configuration.GetSection("ConcurrencyLimiter").Bind(options);
            if (options.Count > 0)
            {
                var option = options.FirstOrDefault(x => x.RequestPath == httpContext.Request.Path);
                if (option != null)
                {
                    var queue = _cacheQueue.GetOrAdd(option.RequestPath, new ActionLimter(option.MaxConcurrentRequests, option.RequestQueueLimit));
                    var result = queue.Doing(() => _nextDelegate?.Invoke(httpContext));
                    if (!result)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        httpContext.Response.WriteAsync(option.QueueFullMessage ?? "The queue is full!", Encoding.UTF8);
                    }
                }
                else
                {
                    _nextDelegate?.Invoke(httpContext);
                }
            }
            else
            {
                _nextDelegate?.Invoke(httpContext);
            }

            return Task.CompletedTask;
        }
    }
}
