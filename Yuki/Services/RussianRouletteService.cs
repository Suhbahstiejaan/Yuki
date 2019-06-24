using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuki.Commands;
using Yuki.Core;

namespace Yuki.Services
{
    public enum RouletteResult
    {
        Killed,
        Safe,
        Not_Current_Player,
        Not_Started,
        No_Game,
        Game_Over,
        Error
    }

    public enum RouletteGameState
    {
        Playing,
        Waiting
    }

    public enum RouletteStartResult
    {
        Starting,
        Not_Enough_Players,
        Not_Game_Master,
        No_Game
    }

    public static class RussianRouletteService
    {
        private static List<RouletteGuild> Guilds = new List<RouletteGuild>();

        public static RouletteGuild GetGuild(ulong guildId)
        {
            if(!Guilds.Any(g => g.Id == guildId))
            {
                Guilds.Add(new RouletteGuild(guildId));
            }

            return Guilds.FirstOrDefault(g => g.Id == guildId);
        }
    }

    public struct RouletteGuild
    {
        public ulong Id;
        public List<RouletteGame> Games;

        public bool GameExists(ulong gameId) => Games.Any(g => g.Id == gameId);

        public RouletteGuild(ulong guildId)
        {
            Id = guildId;
            Games = new List<RouletteGame>();
        }
        
        public RouletteStartResult StartGame(ulong channelId, ulong userId)
        {
            if (GameExists(channelId))
            {
                int indexOfGame = Games.IndexOf(Games.FirstOrDefault(g => g.Id == channelId));
                RouletteGame game = Games[indexOfGame];
                RouletteStartResult result = game.Start(userId);

                Games[indexOfGame] = game;

                return result;
            }
            else return RouletteStartResult.No_Game;
        }

        public bool AddPlayerToGame(ulong channelId, ulong userId)
        {
            if(!GameExists(channelId))
            {
                AddGame(channelId);
            }

            int index = Games.IndexOf(Games.FirstOrDefault(g => g.Id == channelId));
            RouletteGame game = Games[index];
            bool result = game.AddPlayer(userId);

            Games[index] = game;


            return result;
        }

        public void RemovePlayerFromGame(ulong channelId, ulong userId)
        {
            if (GameExists(channelId))
            {
                int index = Games.IndexOf(Games.FirstOrDefault(g => g.Id == channelId));
                RouletteGame game = Games[index];

                game.RemovePlayer(userId);

                Games[index] = game;
            }
        }

        public RouletteResult PullTriggerInGame(ulong channelId, ulong userId)
        {
            if (GameExists(channelId))
            {
                int index = Games.IndexOf(Games.FirstOrDefault(g => g.Id == channelId));
                RouletteGame game = Games[index];
                RouletteResult result = game.PullTrigger(userId);

                Games[index] = game;

                return result;
            }
            else
            {
                return RouletteResult.No_Game;
            }
        }

        public bool KickUserFromGame(ulong channelId, ulong userId, ulong executorId)
        {
            if (GameExists(channelId))
            {
                int index = Games.IndexOf(Games.FirstOrDefault(g => g.Id == channelId));
                RouletteGame game = Games[index];

                if (game.IsGameMaster(executorId))
                {
                    game.RemovePlayer(userId);

                    Games[index] = game;


                    return true;
                }
            }

            return false;
        }


        public void AddGame(ulong channelId)
        {
            if(!Games.Any(g => g.Id == channelId))
            {
                Games.Add(new RouletteGame(channelId));
            }
        }

        public void RemoveGame(ulong channelId)
        {
            if(Games.Any(g => g.Id == channelId))
            {
                Games.Remove(Games[Games.FindIndex(g => g.Id == channelId)]);
            }
        }
    }

    public struct RouletteGame
    {
        public ulong Id;

        public int BulletLocation;
        public int CurrentChamber;

        public RouletteGameState GameState;

        public List<RoulettePlayer> Players;
        

        public RouletteGame(ulong channelId)
        {
            Id = channelId;
            BulletLocation = new YukiRandom().Next(6);
            CurrentChamber = 0;
            GameState = RouletteGameState.Waiting;
            Players = new List<RoulettePlayer>();
        }

        public bool IsGameMaster(ulong userId)
        {
            return Players.IndexOf(Players.FirstOrDefault(p => p.Id == userId)) == 0;
        }

        public bool IsCurrentPlayer(ulong userId)
        {
            return Players[CurrentChamber].Id == userId;
        }

        public RouletteStartResult Start(ulong userId)
        {
            if(IsGameMaster(userId))
            {
                /* CHANGE BEFORE RELEASE */
                if(Players.Count >= 1)
                {
                    GameState = RouletteGameState.Playing;
                    return RouletteStartResult.Starting;
                }
                else
                {
                    return RouletteStartResult.Not_Enough_Players;
                }
            }
            else
            {
                return RouletteStartResult.Not_Game_Master;
            }
        }

        public bool AddPlayer(ulong userId)
        {
            if(!Players.Any(p => p.Id == userId) && Players.Count < 6)
            {
                Players.Add(new RoulettePlayer()
                {
                    Id = userId,
                    TotalLosses = 0,
                    TotalWins = 0,
                    WinRate = 0
                });

                //CheckSetGameMaster();
                return true;
            }

            return false;
        }

        public void RemovePlayer(ulong userId)
        {
            if (Players.Any(p => p.Id == userId))
            {
                Players.Remove(Players.FirstOrDefault(p => p.Id == userId));
                //CheckSetGameMaster();
            }
        }

        /* Game Logic */
        public RouletteResult PullTrigger(ulong userId)
        {
            if (GameState == RouletteGameState.Playing)
            {
                if (IsCurrentPlayer(userId))
                {
                    if (CurrentChamber == BulletLocation)
                    {
                        CurrentChamber = 0;
                        BulletLocation = new YukiRandom().Next(6);
                        RemovePlayer(userId);

                        if(Players.Count > 1)
                        {
                            return RouletteResult.Killed;
                        }
                        else
                        {
                            return RouletteResult.Game_Over;
                        }
                    }
                    else
                    {
                        CurrentChamber++;

                        /* Just to be safe */
                        if(CurrentChamber > 5)
                        {
                            CurrentChamber = 0;
                        }

                        return RouletteResult.Safe;
                    }
                }
                else
                {
                    return RouletteResult.Not_Current_Player;
                }
            }
            else
            {
                return RouletteResult.Not_Started;
            }
        }
    }

    public struct RoulettePlayer
    {
        public ulong Id;
        public int TotalWins;
        public int TotalLosses;
        public double WinRate;
    }
}
