using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTREDParser
{
    class MTREDStats
    {
        // Important Stuff
        public double balance;
        public double unconfirmed;
        public double rsolved;
        
        // Stuff I don't care about
        public double NMCbalance;
        public double NMCunconfirmed;
        public double NMCrsolved;

        public MTREDServerStats server;
        public Dictionary<string, MTREDWorkerStats> workers;

    }

    class MTREDServerStats
    {
        public double hashrate;
        public double workers;
        public double roundshares;
    }

    public class MTREDWorkerStats
    {
        public double mhash;
        public double rsolved;
        public string name;
    }
}
