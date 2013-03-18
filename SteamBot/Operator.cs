
using System;
using SteamKit2;
using SteamTrade;

namespace SteamBot
{
    /// <summary>
    /// A user handler class that implements basic text-based commands entered in
    /// chat or trade chat.
    /// </summary>
    public class Operator : UserHandler
    {
        
        private int currentBotIndex = 0;
        private SteamID lastSID;
        
        
        public Operator(Bot bot, SteamID sid)
            : base(bot, sid)
        {
            //Log.Warn(Configuration.BotInfo[0].Username);
            
        }
        
        #region Overrides of UserHandler
        
        /// <summary>
        /// Called when a the user adds the bot as a friend.
        /// </summary>
        /// <returns>
        /// Whether to accept.
        /// </returns>
        public override bool OnFriendAdd()
        {
            // if the other is an admin then accept add
            if (IsAdmin)
            {
                Log.Warn("Admin SteamID: " + OtherSID + " added the bot as a friend");
                return true;
            }
            if (!(Configuration.BotIDs == null) && (Configuration.BotIDs.Contains (OtherSID)))
            {
                Log.Warn("Bot SteamID: " + OtherSID + " added the bot as a friend");
                return true;
            }
            Log.Warn("Random SteamID: " + OtherSID + " tried to add the bot as a friend");
           return false;
        }
        
        public override void OnFriendRemove()
        {
        }
        
        /// <summary>
        /// Called whenever a message is sent to the bot.
        /// This is limited to regular and emote messages.
        /// </summary>
        public override void OnMessage (string message, EChatEntryType type)
        {
            // TODO: magic command system
            
            if (message == "status")
            {
                if (!IsAdmin)
                    return;
                if (Bot.CurrentTrade == null)
                {
                    Bot.SteamFriends.SendChatMessage (OtherSID, 
                                                      EChatEntryType.ChatMsg,
                                                      "Currently no Trader."
                                                      );
                    return;
                    
                }
                
                if (!Bot.CurrentTrade.TradeStarted)
                {
                    Bot.SteamFriends.SendChatMessage (OtherSID, 
                                                      EChatEntryType.ChatMsg,
                                                      "Trade not started yet."
                                                      );
                    return;
                }
                
                Bot.SteamFriends.SendChatMessage (OtherSID, 
                                                  EChatEntryType.ChatMsg,
                                                  Bot.CurrentTrade.OtherSID + " - To add him just click steam://friends/add/7656" + (Bot.CurrentTrade.OtherSID + 1197960265728) + " ."
                                                  );
                
            }
            if (message == "who am i")
            {
                Bot.SteamFriends.SendChatMessage (OtherSID, 
                                                  EChatEntryType.ChatMsg,
                                                  OtherSID + " or steam://friends/add/7656" + (OtherSID + 1197960265728) + " ."
                                                  );
            }
            
            if (message == "admin wanna help")
            {
                if (!IsAdmin)
                    return;
                if (Bot.CurrentTrade == null)
                    return;
                
                
                if (!Bot.CurrentTrade.TradeStarted)
                    return;
                
                //DONE: what if no trade? -> nothing happens.
                //TODO: what if not admin
                Bot.SteamFriends.SendChatMessage (Bot.CurrentTrade.OtherSID, 
                                                  EChatEntryType.ChatMsg,
                                                  "An Admin is ready to help you. Please Accept Admin's Help."
                                                  
                                                  );
                Bot.SteamFriends.SendChatMessage (Bot.CurrentTrade.OtherSID, 
                                                  EChatEntryType.ChatMsg,
                                                  "To add him just click steam://friends/add/7656" + (OtherSID.AccountID + 1197960265728) + " ."
                                                  );
                
                Bot.SteamFriends.SendChatMessage (OtherSID, 
                                                  EChatEntryType.ChatMsg,
                                                  "Trader Has Been Notified."
                                                  );
            }
            if (message.StartsWith (Messages.MsgShortCmd) || message.StartsWith (Messages.MsgLongCmd))
                HandleMsgCommand (message);
            if (message.StartsWith (Messages.MsgLastCmd))
                HandleReplyMsgCommand (message);
            if (message == Messages.CollectorCollectMsg)
            {
                CollectStep();
            }
        }
        
        void HandleMsgCommand (string message)
        {
            //var data = message.Split(' ');
            if (!IsAdmin)
                return;
            try
            {
                String msgCommand = "";
                if (message.StartsWith (Messages.MsgShortCmd))
                    msgCommand = Messages.MsgShortCmd;
                else //if (message.StartsWith(MsgLongCmd)
                    msgCommand = Messages.MsgLongCmd;
                
                string steamID;
                message = message.Substring (msgCommand.Length + 1); //+1 or not? //,message.Length-MsgShortCmd.Length-1
                
                steamID = message.Substring (0, message.IndexOf (" "));
                message = message.Substring (message.IndexOf (" ") + 1);
                
                SteamID id;
                id = OtherSID;
                if (msgCommand == Messages.MsgShortCmd)
                    id = new SteamID (steamID);
                else if (msgCommand == Messages.MsgLongCmd)
                {
                    ulong SID;
                    if (ulong.TryParse (steamID, out SID))
                    {
                        id = new SteamID (SID);
                    }
                }
                
                
                Bot.SteamFriends.AddFriend (id);
                Bot.SteamFriends.SendChatMessage (id, 
                                                  EChatEntryType.ChatMsg,
                                                  "Admin: "+message
                                                  );
                lastSID = id;
            }
            catch
            {
                // !msg 76561198063518921 ;
                Bot.SteamFriends.SendChatMessage (OtherSID, 
                                                  EChatEntryType.ChatMsg,
                                                  "Message Failed: "+message
                                                  );
            }
            
        }
        
        void HandleReplyMsgCommand (string message)
        {
            if (!IsAdmin)
                return;
            try
            {
                
                
                message = message.Substring (Messages.MsgLastCmd.Length + 1);          
                Bot.SteamFriends.AddFriend (lastSID);
                Bot.SteamFriends.SendChatMessage (lastSID, 
                                                  EChatEntryType.ChatMsg,
                                                  "Admin: "+message
                                                  );
            }
            catch
            {
                Bot.SteamFriends.SendChatMessage (OtherSID, 
                                                  EChatEntryType.ChatMsg,
                                                  "Message Failed: "+message
                                                  );
            }
        }
        
        
        /// <summary>
        /// Called whenever a user requests a trade.
        /// </summary>
        /// <returns>
        /// Whether to accept the request.
        /// </returns>
        public override bool OnTradeRequest()
        {
            Bot.SteamFriends.SendChatMessage (OtherSID, 
                                              EChatEntryType.ChatMsg,
                                              "Hey, you are" + (!IsAdmin?"n't":"") + " Admin."
                                              );
            Bot.SteamFriends.SendChatMessage (OtherSID, 
                                              EChatEntryType.Typing,
                                              ""
                                              );
            Log.Info (String.Format ("Trade Requested from {0}: {1}",
                                     Bot.SteamFriends.GetFriendPersonaName(OtherSID)
                                     ));
            /*if (IsAdmin)*/
            return true;
            
            
        }
        
        public override void OnTradeError(string error)
        {
            Log.Error(error);
        }
        
        public override void OnTradeTimeout()
        {
            Log.Warn("Trade timed out.");
        }
        
        public override void OnTradeInit()
        {
            Trade.SendMessage("Success. (Type " + Messages.HelpCmd + " for commands.)");
        }
        
        public override void OnTradeAddItem(Schema.Item schemaItem, Inventory.Item inventoryItem)
        {
            Trade.SendMessage("You Added: "+schemaItem.Name+" "+schemaItem.CraftClass+" "+schemaItem.Defindex+" - "+(inventoryItem.IsNotCraftable?"not ":"")+"craftable." );
            
            
            // whatever.   
        }
        
        public override void OnTradeRemoveItem(Schema.Item schemaItem, Inventory.Item inventoryItem)
        {
            Trade.SendMessage("You Removed: "+schemaItem.Name+" "+schemaItem.CraftClass+" "+schemaItem.Defindex+" - "+(inventoryItem.IsNotCraftable?"not ":"")+"craftable." );
            
            // whatever.
        }
        
        public override void OnTradeMessage(string message)
        {
            ProcessTradeMessage(message);
        }
        
        public override void OnTradeReady(bool ready)
        {
            Log.Info (String.Format ("Trade Ready from {0}: {1}",
                                     Bot.SteamFriends.GetFriendPersonaName(OtherSID)
                                     ));
            if (!IsAdmin)
            {
                Trade.SendMessage("You are not my master. Thank you for Wasting your Time. LOL.");
                Trade.SetReady(false);
                Log.Error (String.Format ("Trade Ready DENIED from {0}: {1}",
                                          Bot.SteamFriends.GetFriendPersonaName(OtherSID)
                                          ));
                return;
            }
            
            Trade.SetReady(true);
        }
        
        public override void OnTradeAccept()
        {
            Log.Info (String.Format ("Trade Accepted from {0}: {1}",
                                     Bot.SteamFriends.GetFriendPersonaName(OtherSID)
                                     ));
            if (IsAdmin)
            {
                bool ok = Trade.AcceptTrade();
                
                if (ok)
                {
                    Log.Success("Trade was Successful!");
                }
                else
                {
                    Log.Warn("Trade might have failed.");
                }
            }
        }
        
        #endregion
        
        private void ProcessTradeMessage(string message)
        {
            Log.Info (String.Format ("Trade Message from {0}: {1}",
                                     Bot.SteamFriends.GetFriendPersonaName(OtherSID),
                                     message
                                     ));
            if (message.Equals(Messages.HelpCmd))
            {
                PrintHelpMessage();
                return;
            }
            if (message.Equals(Messages.AdminCallCmd))
            {
                CallAdmin();
                return;
            }
            
            if (message.StartsWith(Messages.AddCmd))
                HandleAddCommand(message);
            else if (message.StartsWith(Messages.RemoveCmd))
                HandleRemoveCommand(message);
        }
        
        private void CallAdmin ()
        {
            
            foreach (var adminSID in Bot.Admins)
            {
                Bot.SteamFriends.SendChatMessage (adminSID, 
                                                  EChatEntryType.ChatMsg,
                                                  "Hey, http://steamcommunity.com/profiles/7656"+(OtherSID.AccountID+1197960265728)+ " needs an Admin."
                                                  );
                Bot.SteamFriends.SendChatMessage (adminSID, 
                                                  EChatEntryType.ChatMsg,
                                                  "Add him steam://friends/add/7656"+(Bot.CurrentTrade.OtherSID.AccountID+1197960265728)+ " now."
                                                  );
                lastSID = OtherSID;
                
            }
            
            return;
        }
        
        private void PrintHelpMessage()
        {
            Trade.SendMessage(String.Format(@"{0} - calls an admin to help.", Messages.AdminCallCmd));
            Trade.SendMessage(String.Format("{0} {1} - adds all crates", Messages.AddCmd, Messages.AddCratesSubCmd));
            Trade.SendMessage(String.Format("{0} {1} - adds all metal", Messages.AddCmd, Messages.AddMetalSubCmd));
            Trade.SendMessage(String.Format("{0} {1} - adds all weapons", Messages.AddCmd, Messages.AddWepsSubCmd));
            Trade.SendMessage(String.Format(@"{0} <craft_material_type> [amount] - adds all or a given amount of items of a given crafing type.", Messages.AddCmd));
            Trade.SendMessage(String.Format(@"{0} <defindex> [amount] - adds all or a given amount of items of a given defindex.", Messages.AddCmd));
            
            Trade.SendMessage(@"See http://wiki.teamfortress.com/wiki/WebAPI/GetSchema for info about craft_material_type or defindex.");
        }
        
        private void HandleAddCommand(string command)
        {
            var data = command.Split(' ');
            string typeToAdd;
            
            bool subCmdOk = GetSubCommand (data, out typeToAdd);
            
            if (!subCmdOk)
                return;
            
            uint amount = GetAddAmount (data);
            
            // if user supplies the defindex directly use it to add.
            int defindex;
            if (int.TryParse(typeToAdd, out defindex))
            {
                Trade.AddAllItemsByDefindex(defindex, amount);
                return;
            }
            
            switch (typeToAdd)
            {
            case Messages.AddMetalSubCmd:
                AddItemsByCraftType("craft_bar", amount);
                break;
            case Messages.AddWepsSubCmd:
                AddItemsByCraftType("weapon", amount);
                break;
            case Messages.AddCratesSubCmd:
                AddItemsByCraftType("supply_crate", amount);
                break;
            default:
                AddItemsByCraftType(typeToAdd, amount);
                break;
            }
        }
        
        
        
        private void HandleRemoveCommand(string command)
        {
            var data = command.Split(' ');
            
            string subCommand;
            
            bool subCmdOk = GetSubCommand(data, out subCommand);
            
            if (!subCmdOk)
                return;
        }
        
        
        private void AddItemsByCraftType(string typeToAdd, uint amount)
        {
            var items = Trade.CurrentSchema.GetItemsByCraftingMaterial(typeToAdd);
            
            uint added = 0;
            
            foreach (var item in items)
            {
                added += Trade.AddAllItemsByDefindex(item.Defindex, amount);
                
                // if bulk adding something that has a lot of unique
                // defindex (weapons) we may over add so limit here also
                if (amount > 0 && added >= amount)
                    return;
            }
        }
        
        bool GetSubCommand (string[] data, out string subCommand)
        {
            if (data.Length < 2)
            {
                Trade.SendMessage ("No parameter for cmd: " + data[0]);
                subCommand = null;
                return false;
            }
            
            if (String.IsNullOrEmpty (data [1]))
            {
                Trade.SendMessage ("No parameter for cmd: " + data[0]);
                subCommand = null;
                return false;
            }
            
            subCommand = data [1];
            
            return true;
        }
        
        static uint GetAddAmount (string[] data)
        {
            uint amount = 0;
            
            if (data.Length > 2)
            {
                // get the optional ammount parameter
                if (!String.IsNullOrEmpty (data [2]))
                {
                    uint.TryParse (data [2], out amount);
                }
            }
            
            return amount;
        }
        
        private void CollectStep ()
        {
            if(currentBotIndex>Configuration.BotIDs.ToArray().Length)
                currentBotIndex=0;

            if (Bot.CurrentTrade != null)
                this.Bot.CurrentTrade.CancelTrade ();
            //this.Bot.CloseTrade();
            SteamID id;
            if (Configuration.BotIDs  == null || Configuration.BotIDs.ToArray().Length==0)
            {
                Log.Info ("Please Add the other Bots Steam ID into your Config File because it is not possible to do this in this Program and automized ........ FÜCXING BÜLLSH1T!!!!11ONEONE");
                Bot.SteamFriends.SendChatMessage (OtherSID, 
                                                  EChatEntryType.ChatMsg,
                                                  "Please Add the other Bots Steam ID into your Config File because it is not possible to do this in this Program and automized ........ FÜCXING BÜLLSH1T!!!!11ONEONE"
                                                  );
                return;
            }
            
            id = new SteamID (Configuration.BotIDs.ToArray()[currentBotIndex]);
            Bot.SteamFriends.AddFriend (id);
            Bot.SteamFriends.SendChatMessage (id, 
                                              EChatEntryType.ChatMsg,
                                              Messages.CollectorRequestMsg
                                              );
            Bot.SteamFriends.SendChatMessage (OtherSID, 
                                              EChatEntryType.ChatMsg,
                                              "CollectStep: ["+currentBotIndex+"] id='"+id+"' http://steamcommunity.com/profiles/7656"+(id.AccountID+1197960265728)
                                              );
            Bot.OpenTrade(id);
            //BotControlClass

        }
    }   
}
