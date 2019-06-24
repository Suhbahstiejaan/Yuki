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
                try
                {
                    if (RussianRouletteService.GameExists(Context.Guild.Id, Context.Channel.Id) &&
                        RussianRouletteService.GetGame(Context.Guild.Id, Context.Channel.Id).IsCurrentPlayer(Context.User.Id))
                    {
                        RouletteResult result = RussianRouletteService.GetGuild(Context.Guild.Id).PullTriggerInGame(Context.Channel.Id, Context.User.Id);

                        if (result == RouletteResult.Safe || result == RouletteResult.Game_Over || result == RouletteResult.Killed)
                        {
                            await ReplyAsync(Language.GetString("roulette_trigger_pulled").Replace("%user%", Context.User.Username));

                            await Task.Delay(500);

                            await ReplyAsync(Language.GetString($"roulette_{result.ToString().ToLower()}").Replace("%user%", Context.User.Username));

                            RouletteGame game = RussianRouletteService.GetGame(Context.Guild.Id, Context.Channel.Id);

                            if (game.Players.Count > 1)
                            {
                                await ReplyAsync(Language.GetString("roulette_next_player").Replace("%bullets%", $"{6 - game.CurrentChamber}").Replace("%nuser%", (await Context.Guild.GetUserAsync(game.GetNextPlayer().Id)).Mention));
                            }
                            else
                            {
                                await ReplyAsync(Language.GetString("roulette_winner").Replace("%nuser%", (await Context.Guild.GetUserAsync(game.Players[0].Id)).Mention));

                                RussianRouletteService.GetGuild(Context.Guild.Id).RemoveGame(Context.Channel.Id);
                            }
                        }
                    }
                }
                catch (Exception e) { await ReplyAsync(e); }
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
                    int playersLeft = RouletteGame.CHAMBER_COUNT - RussianRouletteService.GetGame(Context.Guild.Id, Context.Channel.Id).Players.Count;

                    await ReplyAsync(Language.GetString("roulette_join_success").Replace("%spotsleft%", playersLeft.ToString()));
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

                await ReplyAsync(Language.GetString("roulette_game_quit").Replace("%user%", Context.User.Username));
            }

            [Command("kick")]
            public async Task RouletteKickAsync(ulong userId)
            {
                if (RussianRouletteService.GetGuild(Context.Guild.Id).KickUserFromGame(Context.Channel.Id, userId, Context.User.Id))
                {
                    await ReplyAsync(Language.GetString("roulette_player_kicked").Replace("%user%", (await Context.Guild.GetUserAsync(userId)).Mention));
                }
                else
                {
                    await ReplyAsync(Language.GetString("roulette_not_game_master"));
                }
            }
        }
    }
}
