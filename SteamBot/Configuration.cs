using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using SteamKit2;

namespace SteamBot
{
    public class Configuration
    {
        public static Configuration LoadConfiguration (string filename)
        {
            TextReader reader = new StreamReader(filename);
            string json = reader.ReadToEnd();
            reader.Close();

            Configuration config =  JsonConvert.DeserializeObject<Configuration>(json);

            config.Admins = config.Admins ?? new ulong[0];

            // merge bot-specific admins with global admins
            foreach (BotInfo bot in config.Bots)
            {
                if (bot.Admins == null)
                {
                    bot.Admins = new ulong[config.Admins.Length];
                    Array.Copy(config.Admins, bot.Admins, config.Admins.Length);
                }
                else
                {
                    bot.Admins = bot.Admins.Concat(config.Admins).ToArray();
                }
            }
            // merge bot-specific operators with global operators
            foreach (BotInfo bot in config.Bots)
            {
                if (bot.Operators == null)
                {
                    bot.Operators = new ulong[config.Operators.Length];
                    Array.Copy(config.Operators, bot.Operators, config.Operators.Length);
                }
                else
                {
                    bot.Operators = bot.Operators.Concat(config.Operators).ToArray();
                }
            }
            theBots = new List<Bot> ();
            theBotIDs = new List<SteamID> ();


            return config;
        }
        public static List<SteamID> theBotIDs { get; set; }
        public static List<Bot> theBots { get; set; }
        public ulong[] Admins { get; set; }
        public ulong[] Operators { get; set; }
        public BotInfo[] Bots { get; set; }
        public string ApiKey { get; set; }
        public string MainLog { get; set; }

        public class BotInfo
        {
            public bool Disabled { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string DisplayName { get; set; }
            public string ChatResponse { get; set; }
            public string LogFile { get; set; }
            public string BotControlClass { get; set; }
            public int MaximumTradeTime { get; set; }
            public int MaximumActionGap { get; set; }
            public string DisplayNamePrefix { get; set; }
            public int TradePollingInterval { get; set; }
            public string LogLevel { get; set; }
            public ulong[] Admins;
            public ulong[] Operators;
        }
    }
}
