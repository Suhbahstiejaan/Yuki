using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using Yuki.Bot.Common;
using Yuki.Bot.Entity;
using Yuki.Bot.Misc.Extensions;

namespace Yuki.Bot.Commands.User.Fun
{
    public class RussianRoulette
    {
        /* stores the ID of the players in each server */
        public static Dictionary<RouletteServerData, List<ulong>> data = new Dictionary<RouletteServerData, List<ulong>>();

        private RouletteServerData GetServer(ulong guild)
            => data.Keys.FirstOrDefault(x => x.guild == guild);

        public string Add(ulong guild, ulong userId)
        {
            RouletteServerData server = GetServer(guild);

            if(server == null)
            {
                data.Add(new RouletteServerData() { guild = guild, gameHost = userId }, new List<ulong>() { userId });
                return "Joined game";
            }

            if (server != null && !server.isPlaying && !data[server].Contains(userId))
            {
                data[server].Add(userId);
                return "Joined game";
            }
            return "This game has already started";
        }

        public void Remove(ulong guild, ulong userId)
        {
            RouletteServerData server = GetServer(guild);

            if (data[server].Contains(userId))
                data[server].Remove(userId);

            if (server.gameHost == userId)
                server.gameHost = data[server][0];

            Generic.UpdateKey(data, GetServer(guild), server);
        }

        public string GetPlayers(ulong guild, int pageNum)
        {
            SocketGuild _guild = YukiClient.Instance.Client.GetGuild(guild);
            /* Get the username and discriminator value for each player in this game */
            string[] users = data[GetServer(guild)].Select(user => _guild.GetUser(user).Username + "#" + _guild.GetUser(user).Discriminator).ToArray();

            PageManager manager = new PageManager(users, "players");
            
            return manager.GetPage(pageNum);
        }

        public string Start(ulong guild, ulong userId)
        {
            YukiRandom random = new YukiRandom();
            RouletteServerData server = GetServer(guild);
            
            /* Start the game if one exists and the game host runs the command */
            if(server != null)
            {
                if(data[server].Count > 1 && server.gameHost == userId)
                {
                    server.rouletteNumber = random.Next(6);
                    server.isPlaying = true;

                    Generic.UpdateKey(data, GetServer(guild), server);

                    return "*click*\nThe game has been started.\n\nIt's " + YukiClient.Instance.Client.GetGuild(guild).GetUser(data[GetServer(guild)][0]).Mention + "'s turn.";
                }
                return "You're not the host of this game!";
            }
            return "There is no game to start!";
        }

        /* Increase the rounds used and the current player index
         * If the player has died, reset the rounds to zero and put the bullet in a random one
         */
        private string UpdateData(ref RouletteServerData _data, bool isDead = false)
        {
            YukiRandom random = new YukiRandom();

            _data.currentPlayerIndex++;
            _data.currentRoundNumber++;

            if(_data.currentPlayerIndex > data[GetServer(_data.guild)].Count - 1)
                _data.currentPlayerIndex = 0;
            
            if (_data.currentRoundNumber > 5)
                _data.currentRoundNumber = 0;

            if(isDead)
            {
                _data.currentRoundNumber = 0;
                _data.rouletteNumber = random.Next(6);

                if (data[GetServer(_data.guild)].Count != 1)
                    return "The chambers have been reloaded.\n";
            }
            return "";
        }

        /* Main game logic */
        public string Play(ulong guild, ulong user)
        {
            YukiRandom random = new YukiRandom();
            RouletteServerData server = GetServer(guild);

            string username = YukiClient.Instance.Client.GetGuild(guild).GetUser(user).Username;
            string msg = "";

            if (server == null)
                return "There is no game right now!";

            /* Russian Roulette isn't fun if it's only one person - only do game logic if there is more than one player */
            if (data[server].Count > 1)
            {
                /* Quick check to see if the game has started yet */
                if (server.isPlaying)
                {
                    /* Final check to make sure it's the command executor's turn */
                    if (data[server].IndexOf(user) == server.currentPlayerIndex)
                    {
                        /* "Kill" the command executor if their bullet chamber has a bullet */
                        if (server.rouletteNumber == server.currentRoundNumber)
                        {
                            Remove(guild, user);
                            
                            if (data[GetServer(guild)].Count == 1)
                            {
                                server.isPlaying = false;
                                msg += "**GAME OVER!**\n";
                            }

                            msg += ":gun: Sorry, " + username + ", you lose!\n\n";

                            msg += UpdateData(ref server, true);

                            /* End the game if there is a winner */
                            if (data[GetServer(guild)].Count == 1)
                            {
                                msg += "<@" + data[GetServer(guild)][server.currentPlayerIndex] + "> wins!";

                                data.Remove(GetServer(guild));
                            }
                            else
                                msg += "It's <@" + data[GetServer(guild)][server.currentPlayerIndex] + " > 's turn.";
                        }
                        else
                        {
                            UpdateData(ref server);

                            msg = ":eyes: You're safe, " + username + " - for now...\n" + (6 - server.currentRoundNumber) + "/6 chambers left.\n\nIt's <@" + data[GetServer(guild)][server.currentPlayerIndex] + " > 's turn.";
                        }
                    }
                    else
                        msg = "It's not your turn, " + username + "!";
                }
                else
                    msg = "The game hasn't started yet!";
            }
            else
                msg = "Not enough players!";
            
            if(GetServer(guild) != null)
                Generic.UpdateKey(data, GetServer(guild), server);

            return msg;
        }

        /* Remove the player from the current game, if there is one and they are in it */
        public string Leave(ulong guild, ulong user)
        {
            YukiRandom random = new YukiRandom();
            RouletteServerData server = GetServer(guild);

            string username = YukiClient.Instance.Client.GetGuild(guild).GetUser(user).Username;

            if(server != null)
            {
                if(data[server].Contains(user))
                {
                    data[server].Remove(user);

                    if (server.isPlaying)
                    {
                        if (server.rouletteNumber == data[server].IndexOf(user))
                        {
                            server.rouletteNumber = random.Next(6);

                            Generic.UpdateKey(data, GetServer(guild), server);
                        }
                    }


                    if (data[server].Count < 1)
                        data.Remove(server);

                    return "You have left the game.";
                }
                return "You're not in this game!";
            }
            return "There is no game started for this server.";
        }
    }
    
    public class RouletteServerData
    {
        public int rouletteNumber = 0;
        public int currentPlayerIndex = 0;
        public int currentRoundNumber = 0;

        public ulong guild;
        public ulong gameHost;

        public bool isPlaying = false;
    }
}
