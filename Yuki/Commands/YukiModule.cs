using Discord;
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

        public Task ReplyAsync(string content) => Context.ReplyAsync(content);
        public Task ReplyAsync(Embed embed) => Context.ReplyAsync(embed);
        public Task ReplyAsync(EmbedBuilder embed) => Context.ReplyAsync(embed);
        public Task ReactAsync(string unicode) => Context.ReactAsync(unicode);
    }
}
