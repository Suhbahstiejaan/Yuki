using Discord;
using Discord.WebSocket;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationUtilityModule
    {
        [Command("reactionrole")]
        public async Task ReactionRoleAsync([Remainder] string args)
        {
            try
            {
                /* Format: emoji role */
                SocketUserMessage reactionMessage = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 1).FlattenAsync()).FirstOrDefault() as SocketUserMessage;

                string emoteStr = args.Split(' ')[0];
                Emote emote = null;

                Emoji emoji = null;

                if (!(Emote.TryParse(emoteStr, out emote)))
                {
                    emoji = new Emoji(emoteStr);
                }

                IRole role = Context.Guild.Roles.FirstOrDefault(_role => _role.Name.ToLower() == args.Split(' ', 2)[1].ToLower());

                if (reactionMessage == null)
                {
                    await ReplyAsync(Language.GetString("reactionrole_message_needed"));
                    return;
                }

                if ((emote != null || emoji != null) && role != null)
                {
                    ReactionMessage msg = new ReactionMessage()
                    {
                        Id = reactionMessage.Id,
                        Reactions = new List<MessageReaction>()
                    {
                        new MessageReaction()
                        {
                            Emote = emoteStr,
                            RoleId = role.Id
                        }
                    }
                    };

                    GuildSettings.AddReactionMessage(msg, Context.Guild.Id);

                    if (emoji == null)
                    {
                        await reactionMessage.AddReactionAsync(emote);
                    }
                    else
                    {
                        await reactionMessage.AddReactionAsync(emoji);
                    }
                }
            }
            catch(Exception e)
            {
                await ReplyAsync(e);
            }
        }
    }
}
