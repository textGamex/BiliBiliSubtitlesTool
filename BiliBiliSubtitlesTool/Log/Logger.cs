using NLog;
using System;
using System.CommandLine;
using System.Collections.Generic;
using System.Text;

namespace BiliBiliSubtitlesTool.Log
{
    public static class Logger
    {
        private const string LOG_LEVEL = "[off, trace, debug, info, warn, error, fatal]";
        public static void SetLogger(IEnumerable<string> commandArgs)
        {
            if (commandArgs == null)
                throw new ArgumentNullException(nameof(commandArgs));
            var args = new List<string>(commandArgs);
            if (args.Count <= 1)
            {
                return;
            }
            args.RemoveAt(0);
            HandleArgs(args.ToArray());
        }

        private static void HandleArgs(string[] args)
        {
            var fileLogLevelOption = new Option<string>(
            "--logFileOutputLevel-option",
            description: $"日志文件输出等级, 其值可为{LOG_LEVEL}");

            // 将选项添加到根命令
            var rootCommand = new RootCommand
            {
                fileLogLevelOption
            };
            rootCommand.Description = "BiliBiliSubtitleTool";
            rootCommand.SetHandler((string s) =>
            {
                if (s == null)
                {
                    return;
                }
                var level = ParseLevel(s);
                if (level == null)
                    Console.WriteLine($"错误!, 等级只能为{LOG_LEVEL}");
                SetLoggerLevel(level);
            }, fileLogLevelOption);
            rootCommand.Invoke(args);
        }

        private static LogLevel ParseLevel(string level)
        {
            switch (level.ToLower())
            {
                case "off": return LogLevel.Off;
                case "trace": return LogLevel.Trace;
                case "debug": return LogLevel.Debug;
                case "info": return LogLevel.Info;
                case "warn": return LogLevel.Warn;
                case "error": return LogLevel.Error;
                case "fatal": return LogLevel.Fatal;
                default: return null;
            }
        }

        private static void SetLoggerLevel(LogLevel logLevel)
        {
            var config = new NLog.Config.LoggingConfiguration();
            // 登录到的目标：文件和控制台
            var logfile = new NLog.Targets.FileTarget("logfile") 
            {
                FileName = "${basedir}/Logs/${shortdate}.log",
                Encoding = Encoding.UTF8
            };
            //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            logfile.Layout = @"NLog: ${date:format=HH\:mm\:ss} | ${level:uppercase=true:padding=-5} | ${message:exceptionSeparator=\r\n:withException=true}";
            // 将记录器映射到目标的规则
            //config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
            config.AddRule(logLevel, LogLevel.Fatal, logfile);
            // 应用配置
            NLog.LogManager.Configuration = config;
        }

    }
}
