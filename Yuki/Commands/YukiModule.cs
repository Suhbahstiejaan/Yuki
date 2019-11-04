using Discord;
using Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects;

namespace Yuki.Commands
{
    public abstract class YukiModule : ModuleBase<YukiCommandContext>
    {
        public Language Language => Localization.GetLanguage(Context);

        public InteractivityService Interactivity { get; } = YukiBot.Discord.Services.GetRequiredService<InteractivityService>();

        public bool UserHasPriority(IUser executor, IUser otherUser) => Context.UserHasPriority(executor, otherUser);
        public bool RoleHasPriority(IRole role, IRole otherRole) => Context.RoleHasPriority(role, otherRole);
        
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
