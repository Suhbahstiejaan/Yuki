using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Services
{
    public class ReactionRoles
    {
        public static async void Manage(IUserMessage message, ITextChannel channel, SocketReaction reaction, bool isUnreact)
        {
            if (channel is IDMChannel || !reaction.User.IsSpecified)
            {
                return;
            }

            IGuild guild = (channel as IGuildChannel).Guild;

            IGuildUser user = await guild.GetUserAsync(reaction.UserId);

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
                if (r.Emote.ToLower() == reaction.Emote.Name.ToLower())
                {
                    if(isUnreact)
                    {
                        await user.RemoveRoleAsync(guild.GetRole(r.RoleId));
                    }
                    else
                    {
                        await user.AddRoleAsync(guild.GetRole(r.RoleId));
                    }

                    return;
                }
            }
        }
    }
}
