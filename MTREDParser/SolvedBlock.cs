using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTREDParser
{
    class SolvedBlock
    {
        public string hash;
        public DateTime solved_time;
        public string duration;
        public double amount;
        public long shares;
        public bool confirmed;

        /// <summary>
        /// List of all workers and the shares they generated for this block.
        /// </summary>
        public List<MTREDWorkerStats> worker_shares = new List<MTREDWorkerStats>();
    }
}
