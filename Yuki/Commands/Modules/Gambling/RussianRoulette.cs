using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Services;

namespace Yuki.Commands.Modules.Gambling
{
    public partial class GamlingModule
    {
        [Group("roulette")]
        public class RussianRoulette : YukiModule
        {
            [Command]
            public async Task Base()
            {
                RouletteResult result = RussianRouletteService.GetGuild(Context.Guild.Id).PullTriggerInGame(Context.Channel.Id, Context.User.Id);

                await ReplyAsync(Language.GetString($"roulette_{result.ToString().ToLower()}"));
            }

            [Command("start")]
            public async Task RouletteStartAsync()
            {
                RouletteStartResult result = RussianRouletteService.GetGuild(Context.Guild.Id).StartGame(Context.Channel.Id, Context.User.Id);

                await ReplyAsync(Language.GetString($"roulette_{result.ToString().ToLower()}"));
            }

            [Command("join")]
            public async Task RouletteJoinAsync()
            {
                bool result = RussianRouletteService.GetGuild(Context.Guild.Id).AddPlayerToGame(Context.Channel.Id, Context.User.Id);

                if (result)
                {
                    await ReplyAsync(Language.GetString("roulette_join_success"));
                }
                else
                {
                    await ReplyAsync(Language.GetString("roulette_join_fail"));
                }
            }

            [Command("quit")]
            public async Task RouletteQuitAsync()
            {
                RussianRouletteService.GetGuild(Context.Guild.Id).RemovePlayerFromGame(Context.Channel.Id, Context.User.Id);

                await ReplyAsync(Language.GetString("roulette_game_quit"));
            }

            [Command("kick")]
            public async Task RouletteKickAsync(ulong userId)
            {
                if (RussianRouletteService.GetGuild(Context.Guild.Id).KickUserFromGame(Context.Channel.Id, userId, Context.User.Id))
                {
                    await ReplyAsync(Language.GetString("roulette_player_kicked"));
                }
                else
                {
                    await ReplyAsync(Language.GetString("roulette_not_game_master"));
                }
            }
        }
    }
}
