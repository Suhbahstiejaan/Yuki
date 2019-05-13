using Discord.Commands;
using System;
using System.Threading.Tasks;
using Yuki.Bot.Common;

namespace Yuki.Bot.Commands.User.Fun
{
    public partial class Fun_ : ModuleBase
    {
        private RussianRoulette roulette = new RussianRoulette();

        [Command("roulette")]
        public async Task RRouletteAsync([Remainder] string option = "")
        {
            try
            {
                switch (option.ToLower())
                {
                    case "join":
                        await ReplyAsync(roulette.Add(Context.Guild.Id, Context.User.Id));
                        break;
                    case "leave":
                        await ReplyAsync(roulette.Leave(Context.Guild.Id, Context.User.Id));
                        break;
                    case "start":
                        await ReplyAsync(roulette.Start(Context.Guild.Id, Context.User.Id));
                        break;
                    case "players":
                        string[] split = option.Split(' ');
                        int pageNum = 1;

                        if (split.Length > 1)
                            pageNum = int.Parse(split[1]);

                        await ReplyAsync(roulette.GetPlayers(Context.Guild.Id, pageNum));
                        break;
                    default:
                        await ReplyAsync(roulette.Play(Context.Guild.Id, Context.User.Id));
                        break;
                }
            }
            catch (Exception e) { Logger.Instance.Write(LogLevel.Error, e); }
        }
    }
}
