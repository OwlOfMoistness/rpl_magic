using System;
using Discord;
using System.Threading;
using System.Numerics;

namespace RPLBot {
    public class Program {
        public static bool IsRelease = false;
        static void Main(string[] args) {
            Web3Net.RON_RPC = args[2];
            RunBot(token: args[0], prefix: args[1]);
        }
        static void RunBot(string token, string prefix) {
            while (true) {
                try {
                    new Bot().RunAsync(token, prefix).GetAwaiter().GetResult();
                }
                catch (Exception ex) {
                    Logger.Log(new LogMessage(LogSeverity.Error, ex.ToString(), "Unexpected Exception", ex));
                    Console.WriteLine(ex.ToString());
                }
                Thread.Sleep(1000);
                break;
            }
        }
    }
}
