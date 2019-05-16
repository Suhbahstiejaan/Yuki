using System;
using Yuki.Services;

namespace Yuki
{
    public class Program
    {
        private static YukiBot bot = new YukiBot();

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (s, ev) => bot.Shutdown();
            AppDomain.CurrentDomain.ProcessExit += (s, ev) => bot.Shutdown();
            AppDomain.CurrentDomain.UnhandledException += Yuki_UnhandledException;

            Console.Title = "Yuki v" + Version.ToString();

            /* Run the bot */
            bot.LoginAsync().GetAwaiter().GetResult();
        }

        static void Yuki_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LoggingService.Write(LogLevel.Error, e.ExceptionObject);
        }
    }
}
