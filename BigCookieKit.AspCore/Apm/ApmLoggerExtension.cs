
using BigCookieKit;
using BigCookieKit.Algorithm;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace BigCookieKit.AspCore.Apm
{
    public static class ApmLoggerExtension
    {

        internal static string ApmLoggerPath { get; set; }

        public static void UseApmCore<T>(this IApplicationBuilder app, Action<DbContextOptionsBuilder> builder)
            where T : ApmDbContext
        {
            app.UseApmCore(options =>
            {
                options.Add(typeof(T), builder);
            });
        }

        public static void UseApmCore(this IApplicationBuilder app, Action<Dictionary<Type, Action<DbContextOptionsBuilder>>> builders)
        {
            //反向代理的规范
            var forwardHeader = new ForwardedHeadersOptions();
            forwardHeader.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            app.UseForwardedHeaders(forwardHeader);

            app.UseMiddleware<ApmLoggerFilter>();
            app.SQLiteCheck();
            if (Snowflake.WorkId == null) Snowflake.WorkId = 0;

            var options = new Dictionary<Type, Action<DbContextOptionsBuilder>>();
            builders?.Invoke(options);

            foreach (var option in options)
            {
                EsureContextForApm(option.Key);
                ApmDbContext._OnConfiguring.Add(option.Key, new Action<DbContextOptionsBuilder>(options =>
                {
                    var loggerFactory = new LoggerFactory();
                    loggerFactory.AddProvider(new ApmEntityFrameworkLoggerProvider(null, app.ApplicationServices));
                    option.Value?.Invoke(options);
                    options.UseLoggerFactory(loggerFactory);
                    options.EnableSensitiveDataLogging();
                }));
            }
        }

        private static void EsureContextForApm(Type context)
        {
            if (context != null)
            {
                if (context != typeof(ApmDbContext))
                {
                    EsureContextForApm(context.BaseType);
                }
                return;
            }
            throw new Exception("DbContext is not ApmDbContext!");
        }

        private static void SQLiteCheck(this IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetService<IConfiguration>();
            var loggertype = configuration.GetSection("ApmLogger:LoggerType");
            if (loggertype?.Value == "SQLite")
            {
                var loggerpath = configuration.GetSection("ApmLogger:LoggerPath");
                if (loggerpath != null && !string.IsNullOrEmpty(loggerpath?.Value))
                {
                    var absolutePath = Path.GetFullPath(loggerpath?.Value);
                    var apmLoggerPath = Path.Combine(absolutePath, "ApmLogger.db");
                    ApmLoggerPath = apmLoggerPath;
                    if (!Directory.Exists(absolutePath))
                    {
                        Directory.CreateDirectory(absolutePath);
                    }
                    if (!System.IO.File.Exists(apmLoggerPath))
                    {
                        SQLiteConnection.CreateFile(apmLoggerPath);
                        CreateTraceLog();
                    }
                }

            }
        }

        private static void CreateTraceLog()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + ApmLoggerPath))
            {
                if (con.State != ConnectionState.Open) con.Open();
                var cmd = con.CreateCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
CREATE TABLE TraceLogger (
    AppName TEXT,
    TraceId TEXT,
    TraceType TEXT,
    Message TEXT,
    Elapsed TEXT,
    ExecuteTime NUMERIC,
    ExecuteAddress TEXT,
    IsSend NUMERIC
);";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
