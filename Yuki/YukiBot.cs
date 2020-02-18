﻿using Discord;
using Nett;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki
{
    public class YukiBot
    {
        public const string PatronUrl = "https://www.patreon.com/user?u=7361846";
        public const string PayPalUrl = "https://paypal.me/veenus2247";
        public const string ServerUrl = "https://discord.gg/KwHQzuy";
        public const string BotInvUrl = "https://discordapp.com/oauth2/authorize?client_id=338887651677700098&scope=bot&permissions=271690950";
        public const string GithubUrl = "https://github.com/VeeThree/Yuki/";
        public const string WikiUrl   = "https://github.com/VeeThree/Yuki/wiki/";

        public static DiscordBot Discord { get; private set; }
        
        /* Prevent errors on client disconnect */
        public static bool ShuttingDown;

        public YukiBot()
        {
            Logger.Write(LogLevel.Info, "Loading languages....");
            Localization.LoadLanguages();

            FileDirectories.CheckCreateDirectories();

            Discord = new DiscordBot();
        }

        public async Task RunAsync()
        {
            string token;

            if (!File.Exists(FileDirectories.ConfigFile))
            {
                Config c = new Config();

                Console.Write("Please enter your bot's token: ");
                c.token = Console.ReadLine();

                Toml.WriteFile(c, FileDirectories.ConfigFile);
            }

            token = Config.GetConfig(reload: true).token;

            Localization.CheckTranslations();

            await Discord.LoginAsync(token);
            Logger.Write(LogLevel.Info, $"Client has been recommended {Discord.ShardCount} shards");

            Discord.Client.Log += Logger.Write;

            await Discord.Client.StartAsync();

            Logger.Write(LogLevel.Debug, $"Found {Discord.CommandService.GetAllCommands().Count} command(s)");

            UserMessageCache.LoadFromFile();

            await Task.Delay(-1);
        }

        public void Stop()
        {
            ShuttingDown = true;
            Logger.Write(LogLevel.Status, "Stopping client...");

            UserMessageCache.SaveToFile();
            Discord.StopAsync().GetAwaiter().GetResult();

            Thread.Sleep(500);
        }
    }
}
