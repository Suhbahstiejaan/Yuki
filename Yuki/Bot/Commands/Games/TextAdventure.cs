using System;
using System.Collections.Generic;

namespace Yuki.Bot.Modules.Games
{
    public class TextAdventure
    {
        private static Dictionary<ulong, int> usersDictionary = new Dictionary<ulong, int>();
        
        private string noResponse = "I'm sorry I don't understand";

        public TextRoom[] rooms;
        
        public string GetEnterResponse(ulong id) {
            return rooms[usersDictionary[id]].Response;
        }

        public string GetRoomName(ulong id) {
            return rooms[usersDictionary[id]].RoomName;
        }

        public bool UserExists(ulong id)
        {
            return usersDictionary.ContainsKey(id);
        }

        public string GetActionResponse(string actionString, ulong id)
        {
            if(!usersDictionary.ContainsKey(id))
                usersDictionary.Add(id, 0);

            string action = null;

            if(actionString != null)
                action = actionString.Split(new char[] { ' ' }, 2)[0];

            for(int j = 0; j < rooms[usersDictionary[id]].Actions.Length; j++)
            {
                TextRoom room = rooms[usersDictionary[id]];
                bool actionExists = false;
                string actionKey = null;

                if(action != null && room.ActionSynonyms != null)
                {
                    foreach(KeyValuePair<string, string[]> synonym in room.ActionSynonyms)
                    {
                        for(int k = 0; k < synonym.Value.Length; k++)
                        {
                            if(synonym.Value[k] == action && synonym.Key.Contains(actionString.Split(new char[] { ' ' }, 2)[1]))
                            {
                                actionExists = true;
                                actionKey = synonym.Key;
                                break;
                            }
                        }
                        if(actionExists)
                            break;
                    }
                }

                if(room.Actions[j].Split(new char[] { ' ' }, 2)[0] == action || (actionExists && room.Actions[j] == actionKey) || actionString == room.Actions[j])
                {
                    if(j == (rooms[usersDictionary[id]].Actions.Length - 1))
                    {
                        if(usersDictionary[id] != (rooms.Length - 1))
                        {
                            usersDictionary[id]++;
                            return "**" + GetRoomName(id) + "**\n\n" + rooms[usersDictionary[id] - 1].ActionResponses[j] + "\n\n**" + new String('-', 20) + "**\n" + GetEnterResponse(id);
                        }
                        else
                        {
                            return "**" + GetRoomName(id) + "**\n\n" + rooms[usersDictionary[id]].ActionResponses[j] + "\n\nThe end.";
                        }
                    }
                    return "**" + GetRoomName(id) + "**\n\n" + rooms[usersDictionary[id]].ActionResponses[j];
                }
            }

            /* the action they wanted to perform wasnt found */
            if(action != null)
                return noResponse;
            else
                return "**" + GetRoomName(id) + "**\n\n" + GetEnterResponse(id);
        }
    }

    public class TextRoom
    {
        public string RoomName { get; set; }
        public string Response { get; set; }
        public string[] Actions { get; set; }
        public Dictionary<string, string[]> ActionSynonyms { get; set; }
        public string[] ActionResponses { get; set; }
    }
}
