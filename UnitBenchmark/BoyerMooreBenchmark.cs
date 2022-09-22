using BenchmarkDotNet.Attributes;

using BigCookieKit;
using BigCookieKit.Algorithm;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitBenchmark
{
    public class BoyerMooreBenchmark
    {
        byte[] content_byte;
        byte[] pettern_byte;
        char[] content_char;
        char[] pettern_char;
        string content;

        public BoyerMooreBenchmark()
        {
            content = @"Configuration
Overview
RabbitMQ comes with default built-in settings. Those can be entirely sufficient in some environment (e.g. development and QA). For all other cases, as well as production deployment tuning, there is a way to configure many things in the broker as well as plugins.

This guide covers a number of topics related to configuration:

Different ways in which various settings of the server and plugins are configured
Configuration file(s): primary rabbitmq.conf and optional advanced.config
Default configuration file location(s) on various platforms
Configuration troubleshooting: how to find config file location and inspect and verify effective configuration
Environment variables
Operating system (kernel) limits
Available core server settings
Available environment variables
How to encrypt sensitive configuration values
and more.

Since configuration affects many areas of the system, including plugins, individual documentation guides dive deeper into what can be configured. Runtime Tuning is a companion to this guide that focuses on the configurable parameters in the runtime. Production Checklist is a related guide that outlines what settings will likely need tuning in most production environments.";
            //string pettern = "effective config";
            //string pettern = "configuration";
            string pettern = "config";

            content_byte = Encoding.Default.GetBytes(content);
            pettern_byte = Encoding.Default.GetBytes(pettern);

            content_char = content.ToCharArray();
            pettern_char = pettern.ToCharArray();
        }

        [Benchmark]
        public void BoyerMooreByte()
        {
            BoyerMoore.BoyerMooreFirstMatch(content_byte, pettern_byte);
        }

        [Benchmark]
        public void BoyerMooreChar()
        {
            var s1 = BoyerMoore.BoyerMooreFirstMatch(content_char, pettern_char);
            var arr = BoyerMoore.BoyerMooreMatchAll(content_char, pettern_char);
        }
    }
}
