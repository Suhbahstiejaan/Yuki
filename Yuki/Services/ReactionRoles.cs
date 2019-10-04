using Discord;
using Discord.WebSocket;
using System.Linq;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Services
{
    public class ReactionRoles
    {
        public static async void ManageReact(IUserMessage message, ITextChannel channel, SocketReaction reaction)
        {
            if (channel is IDMChannel)
            {
                return;
            }


            IGuild guild = (channel as IGuildChannel).Guild;

            IGuildUser user = await guild.GetUserAsync(message.Author.Id);

            GuildConfiguration config = GuildSettings.GetGuild(guild.Id);
            
            if (config.Equals(null) || !config.EnableReactionRoles)
            {
                return;
            }

            ReactionMessage reactionMessage = config.ReactableMessages.FirstOrDefault(_message => _message.Id == reaction.MessageId);

            if (reactionMessage.Equals(default) || reactionMessage.Reactions == null)
            {
                return;
            }

            foreach (MessageReaction r in reactionMessage.Reactions)
            {
                Emote emote = null;

                Emoji emoji = null;

                if (!Emote.TryParse(r.Emote, out emote))
                {
                    emoji = new Emoji(r.Emote);
                }

                if (emoji != null || emote != null)
                {
                    await user.AddRoleAsync(guild.GetRole(r.RoleId));
                    return;
                }
            }
        }

        public static async void ManageUnreact(IUserMessage message, ITextChannel channel, SocketReaction reaction)
        {
            if (channel is IDMChannel)
            {
                return;
            }


            IGuild guild = (channel as IGuildChannel).Guild;

            IGuildUser user = await guild.GetUserAsync(message.Author.Id);

            GuildConfiguration config = GuildSettings.GetGuild(guild.Id);

            if (config.Equals(null) || !config.EnableReactionRoles)
            {
                return;
            }

            ReactionMessage reactionMessage = config.ReactableMessages.FirstOrDefault(_message => _message.Id == reaction.MessageId);

            if (reactionMessage.Equals(default) || reactionMessage.Reactions == null)
            {
                return;
            }

            foreach (MessageReaction r in reactionMessage.Reactions)
            {
                Emote emote = null;

                Emoji emoji = null;

                if (!Emote.TryParse(r.Emote, out emote))
                {
                    emoji = new Emoji(r.Emote);
                }

                if (emoji != null || emote != null)
                {
                    await user.RemoveRoleAsync(guild.GetRole(r.RoleId));
                    return;
                }
            }
        }
    }
}
