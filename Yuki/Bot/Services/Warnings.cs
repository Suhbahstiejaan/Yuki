using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Discord.Events;

namespace Yuki.Bot.Services
{
    public enum WarningAction
    {
        BAN,
        KICK,
        APPLYROLE
    }

    public class Warnings
    {
        GuildEvents events = new GuildEvents();

        public async Task Act(WarnedUser user)
        {
            try
            {
                using(UnitOfWork uow = new UnitOfWork())
                {
                    GuildWarningAction action = uow.WarningActionRepository.GetAction(user.ServerId, user.Warning);

                    SocketGuild guild = YukiClient.Instance.DiscordClient.GetGuild(user.ServerId);
                    IGuildUser guser = guild.GetUser(user.UserId);
                    
                    switch(action.Action)
                    {
                        case WarningAction.BAN:
                            await guild.AddBanAsync(guser);
                            await events.UserBanned((SocketUser)guser, YukiClient.Instance.DiscordClient.CurrentUser, guild, "User reached " + user.Warning + " warnings");
                            break;
                        case WarningAction.KICK:
                            await guser.KickAsync();
                            await events.UserKicked((SocketUser)guser, YukiClient.Instance.DiscordClient.CurrentUser, guild, "User reached " + user.Warning + " warnings");
                            break;
                        case WarningAction.APPLYROLE:
                            if(action.RoleId != 0)
                            {
                                IRole role = guild.GetRole(action.RoleId);
                                await guser.AddRoleAsync(role);
                            }
                            break;
                    }
                }
            }
            catch(Exception e) { Console.WriteLine(e); }
        }
    }
}
