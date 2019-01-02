using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Yuki.Bot.Misc.Database;
using Yuki.Bot.Discord.Events;

namespace Yuki.Bot.Services
{
    public class MuteService
    {
        private static List<MutedUser> mutedUsers = new List<MutedUser>();

        private GuildEvents events = new GuildEvents();

        public MutedUser GetUser(ulong id)
            => mutedUsers.FirstOrDefault(x => x.Id == id);

        public async void Mute(MutedUser user, SocketUser moderator)
        {
            using(UnitOfWork uow = new UnitOfWork())
            {
                await (await user.Guild.GetUserAsync(user.Id)).AddRoleAsync(user.Guild.GetRole(uow.MuteRolesRepository.GetMuteRole(user.Guild.Id).RoleId));
                mutedUsers.Add(user);

                await events.UserMute((SocketUser)await user.Guild.GetUserAsync(user.Id), moderator, (SocketGuild)user.Guild, user.Time, user.MuteReason);
            }
        }

        public async void Unmute(MutedUser user, SocketUser moderator)
        {
            using(UnitOfWork uow = new UnitOfWork())
            {
                mutedUsers.Remove(user);
                user.Timer.Stop();
                await user.MuteChannel.SendMessageAsync(user.Guild.GetUserAsync(user.Id).Result.Username + " has been unmuted");
                await (await user.Guild.GetUserAsync(user.Id)).RemoveRoleAsync(user.Guild.GetRole(uow.MuteRolesRepository.GetMuteRole(user.Guild.Id).RoleId));
                await events.UserUnmute((SocketUser)(await user.Guild.GetUserAsync(user.Id)), moderator, (SocketGuild)user.Guild, user.Time, user.MuteReason);
            }
        }

        public async void Unmute(ulong userId)
        {
            MutedUser user = mutedUsers.FirstOrDefault(x => x.Id == userId);

            using(UnitOfWork uow = new UnitOfWork())
            {
                mutedUsers.Remove(user);
                user.Timer.Stop();
                await user.MuteChannel.SendMessageAsync(user.Guild.GetUserAsync(user.Id).Result.Username + " has been unmuted");
                await (await user.Guild.GetUserAsync(user.Id)).RemoveRoleAsync(user.Guild.GetRole(uow.MuteRolesRepository.GetMuteRole(user.Guild.Id).RoleId));
            }
        }
    }

    public class MutedUser
    {
        public ulong Id { get; set; }
        public IGuild Guild { get; set; }
        public ITextChannel MuteChannel { get; set; }
        public TimeSpan Time { get; set; }
        public Timer Timer { get; set; }
        public string MuteReason { get; set; }
    }
}
