using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTREDParser
{
    /* {
     *  "balance":"0.60130465",
     *  "unconfirmed":"0.08000939",
     *  "rsolved":"566",
     *  "NMCbalance":"0.00000000",
     *  "NMCunconfirmed":"0.00000000",
     *  "NMCrsolved":"0",
        "server": {
     *              "hashrate":156.7663,
     *              "workers":352,
     *              "roundshares":428848,
     *              "NMCroundshares":"720060",
     *              "servers": {
     *                              "n2": {
     *                                      "id":2,
     *                                      "host":"us.mtred.com",
     *                                      "hashrate":156.7663,
     *                                      "workers":352,
     *                                      "roundshares":428848,
     *                                      "NMCroundshares":"720060"
     *                                     }
     *                               },
     *                              "foundblock":0,
     *                              "foundNMCblock":0
     *              },
     *  "workers": {
     *                              "lologarithm_w1": {
     *                                                  "mhash":42.94967296,
     *                                                  "rsolved":"196"
     *                                                },
     *                              "lologarithm_w2": {
     *                                                  "mhash":143.16557653333,"rsolved":"370"},"jimmy90":{"mhash":0,"rsolved":"0"
     *                                                 }
     *                          }
     *              }
     */

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
