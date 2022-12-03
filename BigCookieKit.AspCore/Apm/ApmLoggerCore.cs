using System.Data.SQLite;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Net;
using System.Reflection.PortableExecutable;

namespace BigCookieKit.AspCore.Apm
{
    public static class ApmLoggerCore
    {
        private static readonly Dictionary<string, string> Mapping = new Dictionary<string, string>()
        {
            { "elapsed" , "执行时间(ms)" },
            { "parameters" , "参数" },
            { "commandType" , "SQL类型" },
            { "commandTimeout" , "SQL超时时间(m)" },
            { "commandText" , "SQL语句" },
        };

        public static void ConsoleApiLogger(HttpContext context, Stopwatch stopwatch, string message)
        {
            var configuration = context.RequestServices.GetService<IConfiguration>();
            var appName = configuration.GetSection("ApmLogger:AppName")?.Value;
            var tarceId = context.Request.Headers["TraceId"];

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"**********************[{appName}:Api执行信息]**********************");
            Console.WriteLine($"TrackId:{tarceId}");
            Console.WriteLine($"Api路径:{context?.Request?.Path}");
            Console.WriteLine($"Api请求参数:{context.FormatApiRequestMessage()}");
            Console.WriteLine($"Api响应参数:{context.FormatApiResponseMessage()}");
            Console.WriteLine($"Api执行时间:{stopwatch.ElapsedMilliseconds}ms");
            if (!string.IsNullOrEmpty(message))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"异常信息:{message}");
            }
            Console.ResetColor();
        }

        public static void SQLiteApiLogger(HttpContext context, Stopwatch stopwatch, string message)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + ApmLoggerExtension.ApmLoggerPath))
            {
                var configuration = context.RequestServices.GetService<IConfiguration>();
                var appName = configuration.GetSection("ApmLogger:AppName")?.Value;
                var tarceId = context.Request.Headers["TraceId"];
                var ipAddress = context.Connection.RemoteIpAddress.MapToIPv4()?.ToString();

                if (con.State != ConnectionState.Open) con.Open();
                var cmd = con.CreateCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
INSERT INTO TraceLogger(AppName,TraceId,TraceType,Message,Elapsed,ExecuteTime,ExecuteAddress,IsSend)
VALUES(@AppName,@TraceId,@TraceType,@Message,@Elapsed,@ExecuteTime,@ExecuteAddress,@IsSend);";
                cmd.Parameters.Add(new SQLiteParameter("AppName", appName));
                cmd.Parameters.Add(new SQLiteParameter("TraceId", tarceId));
                cmd.Parameters.Add(new SQLiteParameter("TraceType", "api"));
                if (!string.IsNullOrEmpty(message))
                {
                    cmd.Parameters.Add(new SQLiteParameter("Message", message));
                }
                else
                {
                    cmd.Parameters.Add(new SQLiteParameter("Message", $"Path:{context.Request.Path} Request:{context.FormatApiRequestMessage()} Response:{context.FormatApiResponseMessage()}"));
                }
                cmd.Parameters.Add(new SQLiteParameter("Elapsed", $"{stopwatch.ElapsedMilliseconds}ms"));
                cmd.Parameters.Add(new SQLiteParameter("ExecuteTime", DateTime.Now));
                cmd.Parameters.Add(new SQLiteParameter("ExecuteAddress", ipAddress));
                cmd.Parameters.Add(new SQLiteParameter("IsSend", false));
                cmd.ExecuteNonQuery();
            }
        }

        public static void ConsoleEFLogger(HttpContext context, Dictionary<string, object?> values, Exception exception)
        {
            var configuration = context.RequestServices.GetService<IConfiguration>();
            var appName = configuration.GetSection("ApmLogger:AppName")?.Value;
            var tarceId = context.Request.Headers["TraceId"];
            Console.WriteLine();
            if (exception == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"**********************[{appName}:SQL执行信息]**********************");
                Console.WriteLine($"TrackId:{tarceId}");
                foreach (var item in values)
                {
                    if (Mapping.ContainsKey(item.Key))
                    {
                        Console.WriteLine($"{Mapping[item.Key]}:{item.Value}");
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"**********************[{appName}:SQL异常信息]**********************");
                Console.WriteLine($"异常信息:{exception.Message}");
            }
            Console.ResetColor();
        }

        public static void SQLiteEFLogger(HttpContext context, Dictionary<string, object> values, Exception exception)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=" + ApmLoggerExtension.ApmLoggerPath))
            {
                var configuration = context.RequestServices.GetService<IConfiguration>();
                var appName = configuration.GetSection("ApmLogger:AppName")?.Value;
                var tarceId = context.Request.Headers["TraceId"];
                var ipAddress = context.Connection.RemoteIpAddress.MapToIPv4()?.ToString();
                var message = exception == null ? JsonSerializer.Serialize(values) : exception.Message;

                if (con.State != ConnectionState.Open) con.Open();
                var cmd = con.CreateCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
INSERT INTO TraceLogger(AppName,TraceId,TraceType,Message,Elapsed,ExecuteTime,ExecuteAddress,IsSend)
VALUES(@AppName,@TraceId,@TraceType,@Message,@Elapsed,@ExecuteTime,@ExecuteAddress,@IsSend);";
                cmd.Parameters.Add(new SQLiteParameter("AppName", appName));
                cmd.Parameters.Add(new SQLiteParameter("TraceId", tarceId));
                cmd.Parameters.Add(new SQLiteParameter("TraceType", "db"));
                cmd.Parameters.Add(new SQLiteParameter("Message", message));
                cmd.Parameters.Add(new SQLiteParameter("Elapsed", $"{values["elapsed"]}ms"));
                cmd.Parameters.Add(new SQLiteParameter("ExecuteTime", DateTime.Now));
                cmd.Parameters.Add(new SQLiteParameter("ExecuteAddress", ipAddress));
                cmd.Parameters.Add(new SQLiteParameter("IsSend", false));
                cmd.ExecuteNonQuery();
            }
        }

        private static string FormatApiRequestMessage(this HttpContext context)
        {
            var message = new Dictionary<string, object>();

            if (context.Request.Query != null && context.Request.Query.Count > 0)
            {
                message.Add("Query", context.Request.Query);
            }

            using var reader = new StreamReader(context.Request.Body);
            var body = reader.ReadToEndAsync().Result;
            if (!string.IsNullOrEmpty(body))
            {
                message.Add("Body", body);
            }

            if (context.Request.Headers.ContainsKey("Content-Type"))
            {
                if (context.Request.Headers["Content-Type"] == "application/x-www-form-urlencoded"
                    || context.Request.Headers["Content-Type"] == "multipart/form-data")
                {
                    if (context.Request.Form != null && context.Request.Form.Count > 0)
                    {
                        message.Add("Form", context.Request.Form);
                    }
                }
            }

            return JsonSerializer.Serialize(message);
        }

        private static string FormatApiResponseMessage(this HttpContext context)
        {
            string body = string.Empty;
            context.Response.Body.Position = 0;
            if (!string.IsNullOrEmpty(context.Response.ContentType)
                && context.Response.ContentType.Contains("application/json"))
            {
                var reader = new StreamReader(context.Response.Body);
                body = reader.ReadToEndAsync().Result;
                context.Response.Body.Position = 0;
            }
            return body;
        }
    }
}
