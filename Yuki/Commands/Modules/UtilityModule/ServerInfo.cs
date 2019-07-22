using Discord;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Core;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("serverinfo")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ServerInfoAsync()
        {
            IGuild guild = Context.Guild;

            IGuildUser guildOwner = await guild.GetOwnerAsync();

            int voiceCount = (await guild.GetVoiceChannelsAsync()).Count;
            int textCount = (await guild.GetTextChannelsAsync()).Count;

            string channels = textCount + " " + Language.GetString("serverinfo_channels_text") + "\n" +
                              voiceCount + " " + Language.GetString("serverinfo_channels_voice") + "\n\n" +
                              (await guild.GetCategoriesAsync()).Count + " " + Language.GetString("serverinfo_categories") + "\n";

            Embed embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                {
                    IconUrl = guild.IconUrl,
                    Name = guild.Name
                })
                .WithThumbnailUrl(guild.IconUrl)
                .WithColor(Colors.Pink)
                .AddField(Language.GetString("serverinfo_owner"), $"{guildOwner.Username}#{guildOwner.Discriminator} ({guildOwner.Id})")
                .AddField("ID", guild.Id, true)
                .AddField("Shard", Context.Client.GetShardFor(guild).ShardId, true)
                .AddField(Language.GetString("serverinfo_region"), guild.VoiceRegionId, true)
                .AddField(Language.GetString("serverinfo_verification_level"), guild.VerificationLevel, true)
                .AddField(Language.GetString("serverinfo_channels") + $"[{textCount + voiceCount}]", channels, true)
                .AddField(Language.GetString("serverinfo_members") + $"[{(await guild.GetUsersAsync()).Count}]", $"{(await guild.GetUsersAsync()).Where(user => user.Status != UserStatus.Offline).Count()} {Language.GetString("serverinfo_online")}", true)
                .AddField(Language.GetString("serverinfo_roles") + $"[{guild.Roles.Count}]", Language.GetString("serverinfo_roles_view"), true)
                .WithFooter(Language.GetString("serverinfo_created") + ": " + guild.CreatedAt.DateTime.ToPrettyTime(false, false))
                .Build();

            await ReplyAsync(embed);
        }
    }
}
