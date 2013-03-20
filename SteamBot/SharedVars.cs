using System;
using System.Collections.Generic;
using SteamKit2;

namespace SteamBot
{
    public static class SharedVars
    {
        public static List<SteamID> BotIDs = new List<SteamID>();
        
        public static void AddBotToList(SteamID id)
        {
            BotIDs.Add(id);
        }
        public static bool IsBotOnList(SteamID id)
        {
            return BotIDs.Contains(id);
        }
        public static TradeStati theStatus;

    }
}

