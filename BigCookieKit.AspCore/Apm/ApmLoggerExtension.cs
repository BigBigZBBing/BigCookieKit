
using BigCookieKit;
using BigCookieKit.Algorithm;

using Microsoft.AspNetCore.Builder;
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
            app.UseApmCore(new Dictionary<Type, Action<DbContextOptionsBuilder>>()
            {
                { typeof(T) , builder }
            });
        }

        public static void UseApmCore(this IApplicationBuilder app, Dictionary<Type, Action<DbContextOptionsBuilder>> builders)
        {
            app.UseMiddleware<ApmLoggerFilter>();
            app.SQLiteCheck();
            if (Snowflake.WorkId == null) Snowflake.WorkId = 0;

            foreach (var builder in builders)
            {
                var _config = GetContextOnConfiguring(builder.Key);
                _config?.SetValue(null, new Action<DbContextOptionsBuilder>(options =>
                {
                    var loggerFactory = new LoggerFactory();
                    loggerFactory.AddProvider(new ApmEntityFrameworkLoggerProvider(null, app.ApplicationServices));
                    builder.Value?.Invoke(options);
                    options.UseLoggerFactory(loggerFactory);
                    options.EnableSensitiveDataLogging();
                }));
            }
        }

        private static PropertyInfo GetContextOnConfiguring(Type context)
        {
            if (context != null)
            {
                if (context != typeof(ApmDbContext))
                {
                    return GetContextOnConfiguring(context.BaseType);
                }
                else
                {
                    return context.GetProperty("_OnConfiguring");
                }
            }
            return null;
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
    TraceType NUMERIC,
    Message NUMERIC,
    Elapsed NUMERIC,
    ExecuteTime NUMERIC,
    IsSend NUMERIC
);";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
