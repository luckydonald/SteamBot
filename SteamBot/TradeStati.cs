using System;
using SteamKit2;

namespace SteamBot
{
    public class TradeStati
    {
        public TradeStati(SteamID ownSID,SteamID otherSID){
            thisBot = new TradeStatus(ownSID);
            otherBot = new TradeStatus(otherSID);
            isInUse=false;
        }
        public TradeStatus thisBot;
        public TradeStatus otherBot;
        private bool isInUse;

        public bool IsInUse
        {
            get
            {
                return isInUse;
            }
            set
            {
                isInUse = value;
            }
        }

        //////////////////////////////////////////////////////////////
        public class TradeStatus{
            public TradeStatus(SteamID sid){
                id=sid;
                inited=false;
            }

            private SteamID id;
            private bool inited;
            private bool ready;


            public SteamID Id
            {
                get
                 {
                    return id;
                }
                set
                {
                    id = value;
                }
            }
            public bool Inited
            {
                get
                {
                    return inited;
                }
                set
                {
                    inited = value;
                }
            }

            public bool Ready
            {
                get
                {
                    return ready;
                }
                set
                {
                    ready = value;
                }
            }
        }
        
    }
}

