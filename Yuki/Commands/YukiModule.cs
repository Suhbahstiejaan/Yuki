﻿using Discord;
using InteractivityAddon;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Services;

namespace Yuki.Commands
{
    public abstract class YukiModule : ModuleBase<YukiCommandContext>
    {
        public Language Language => LocalizationService.GetLanguage(Context);

        public InteractivityService Interactivity { get; } = YukiBot.Services.GetRequiredService<InteractivityService>();

        public Task<IUserMessage> ReplyAsync(string content, Embed embed) => Context.ReplyAsync(content, embed);
        public Task<IUserMessage> ReplyAsync(string content, EmbedBuilder embed) => Context.ReplyAsync(content, embed);
        public Task<IUserMessage> ReplyAsync(string content) => Context.ReplyAsync(content);
        public Task<IUserMessage> ReplyAsync(object content) => Context.ReplyAsync(content);
        public Task<IUserMessage> ReplyAsync(Embed embed) => Context.ReplyAsync(embed);
        public Task<IUserMessage> ReplyAsync(EmbedBuilder embed) => Context.ReplyAsync(embed);
        public Task<IUserMessage> SendFileAsync(string file, EmbedBuilder embed) => Context.SendFileAsync(file, embed);
        public Task<IUserMessage> SendFileAsync(string file, Embed embed) => Context.SendFileAsync(file, embed);
        public Task ReactAsync(string unicode) => Context.ReactAsync(unicode);
    }
}
