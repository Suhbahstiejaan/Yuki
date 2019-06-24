﻿using Discord;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Data.Objects;
using Yuki.Data.Objects.API;
using Yuki.Services;

namespace Yuki.API
{
    public static class RamMoe
    {
        public static async Task<string> GetImageAsync(string type, bool isNsfw = false)
        {
            using (HttpClient http = new HttpClient())
            {
                string url = "https://rra.ram.moe/i/r?type=" + type + "&nsfw=" + isNsfw.ToString().ToLower();

                /* Read the json, return the received string */
                using (StreamReader reader = new StreamReader(await http.GetStreamAsync(url)))
                {
                    RamMoeFile moe = JsonConvert.DeserializeObject<RamMoeFile>(reader.ReadToEnd());

                    return "https://cdn.ram.moe/" + moe.path.Remove(0, 3);
                }
            }
        }

        public static async Task SendImageAsync(YukiCommandContext context, Language lang, string imgType, bool isNsfw, params IUser[] mentionedUser)
        {
            try
            {
                string embedStringTitle = "rammoe_" + imgType;

                if ((mentionedUser == null || mentionedUser.Length == 0) && lang.GetString(embedStringTitle + "_alt") != embedStringTitle + "_alt")
                {
                    embedStringTitle += "_alt";
                    mentionedUser = new IUser[]
                    {
                        context.Client.CurrentUser
                    };
                }

                string translatedTitle = lang.GetString(embedStringTitle)
                    .Replace("%executor%", context.User.Username)
                    .Replace("%user%", ((mentionedUser != null) || mentionedUser.Length != 0) ? string.Join(", ", mentionedUser.Select(u => u.Username)) : "");

                await context.ReplyAsync(context.CreateImageEmbedBuilder(translatedTitle, await GetImageAsync(imgType, isNsfw)));
            }
            catch (Exception e)
            {
                LoggingService.Write(LogLevel.Debug, e);
            }
        }
    }
}