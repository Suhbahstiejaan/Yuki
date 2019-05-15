using System;

namespace Yuki
{
    public class Program
    {
        private static YukiBot bot = new YukiBot();

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (s, ev) => bot.Shutdown();
            AppDomain.CurrentDomain.ProcessExit += (s, ev) => bot.Shutdown();

            Console.Title = "Yuki v" + Version.ToString();

            /* Run the bot */
            bot.LoginAsync().GetAwaiter().GetResult();
        }
    }
}
