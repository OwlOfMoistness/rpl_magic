﻿using System;
using System.Threading.Tasks;
using Discord;
namespace RPLBot {
    public static class Logger {
        private static readonly ThreadSafeFileBuffer<string> LogWriter = new ThreadSafeFileBuffer<string>("Logger/log.txt"); // server
        public static void Log(string message) {
            try {
                Console.WriteLine(message);
            }
            catch (Exception) {
            }
        }

        public static void LogInternal(string message) {
            try {
                var cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Gray;
                string value = $"{DateTime.Now,-19} [    Info] " + message;
                Console.WriteLine(value);
                Console.ForegroundColor = cc;
                LogWriter.WriteLine(value);
            }
            catch (Exception) {
            }
        }

        public static Task Log(LogMessage message) {
            try {
                var cc = Console.ForegroundColor;
                switch (message.Severity) {
                    case LogSeverity.Critical:
                    case LogSeverity.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogSeverity.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogSeverity.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogSeverity.Verbose:
                    case LogSeverity.Debug:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                string output = $"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message}";
                Console.WriteLine(output);
                Console.ForegroundColor = cc;

                if (message.Exception != null) {
                    output += $"\r\nException:\r\n{message.Exception}";
                    if (message.Exception.InnerException != null) {
                        output += $"\r\nInnerException:\r\n{message.Exception.InnerException}";
                    }
                }
                LogWriter.WriteLine(output);
            }
            catch (Exception) {
            }
            return Task.CompletedTask;
        }
    }
}
