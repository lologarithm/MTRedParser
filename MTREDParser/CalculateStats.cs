using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Threading;

namespace MTREDParser
{
    class CalculateStats
    {
        private MainWindow Window;
        private Dictionary<long, List<MTREDWorkerStats>> stats_dictionary = new Dictionary<long, List<MTREDWorkerStats>>();
        public bool run = true;

        public CalculateStats(MainWindow i_window)
        {
            this.Window = i_window;
        }

        public void DoStats(object api_key)
        {

            while (run)
            {

                var json = new WebClient().DownloadString("https://mtred.com/api/user/key/" + (string)api_key);

                MTREDStats values = JsonConvert.DeserializeObject<MTREDStats>(json);

                List<MTREDWorkerStats> worker_stats = new List<MTREDWorkerStats>();
                foreach (string key in values.workers.Keys)
                {
                    MTREDWorkerStats stats = values.workers[key];
                    stats.name = key;

                    worker_stats.Add(stats);
                }



                Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<List<MTREDWorkerStats>>(Window.AddListItem), worker_stats);


                Thread.Sleep(1000);


            }
        }
    }
}
