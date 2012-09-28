using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Threading;
using HtmlAgilityPack;
using System.IO;

namespace MTREDParser
{
    class CalculateStats
    {
        private MainWindow Window;
        
        // History of all data points (every 30 seconds).
        private Dictionary<long, List<MTREDWorkerStats>> data_history = new Dictionary<long, List<MTREDWorkerStats>>();
        // List of solved blocks and the amount of work each worker put into this block.
        private Dictionary<string, SolvedBlock> solved_blocks = new Dictionary<string, SolvedBlock>();

        private string APIKey = "";
        private long previous_data_history = 0;

        public bool run = true;

        public Timer StatTimer = null;

        public CalculateStats(MainWindow i_window)
        {
            this.Window = i_window;
        }

        public void DoStats(object api_key)
        {
            this.APIKey = (string)api_key;

            if (run)
            {
                // 1. Get list of solved blocks

                HtmlWeb web = new HtmlWeb();
                HtmlDocument block_page = web.Load("https://mtred.com/blocks.html");
                HtmlNodeCollection row_nodes = block_page.DocumentNode.SelectNodes("//table[contains(@class, 'items')]/tbody/tr");

                foreach (HtmlNode row_node in row_nodes)
                {
                    HtmlNodeCollection row_parts = row_node.SelectNodes("td");
                    HtmlNode block_hash_node = row_parts[1];
                    string hash = row_parts[1].SelectSingleNode("a[@href]").GetAttributeValue("href", string.Empty).ToString().Split('/')[4];
                    long solved_time = long.Parse(row_parts[2].InnerText);
                    string duration = row_parts[3].InnerText;
                    double value = double.Parse(row_parts[4].InnerText);
                    long total_shares = long.Parse(row_parts[5].InnerText);
                    if ( !solved_blocks.ContainsKey(hash) )
                    {
                        SolvedBlock sb = new SolvedBlock();
                        sb.amount = value;
                        sb.hash = hash;
                        sb.shares = total_shares;
                        sb.solved_time = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(solved_time).ToLocalTime();
                        sb.duration = duration;
                        // TODO: make this work properly... blah

                        if (previous_data_history > 0)
                        {
                            sb.worker_shares = data_history[previous_data_history];
                            Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<List<MTREDWorkerStats>>(Window.FillListBox), sb.worker_shares);
                        }

                        solved_blocks.Add(hash, sb);
                        Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<MTREDWorkerStats>(Window.AddListBox), new MTREDWorkerStats() { rsolved = sb.shares, name = "Total: " });
                    }
                }

                // 2. Get newest data point.
                long current_time = DateTime.Now.ToFileTimeUtc();
                var json = new WebClient().DownloadString("https://mtred.com/api/user/key/" + APIKey);

                MTREDStats values = JsonConvert.DeserializeObject<MTREDStats>(json);

                List<MTREDWorkerStats> worker_stats = new List<MTREDWorkerStats>();
                foreach (string key in values.workers.Keys)
                {
                    MTREDWorkerStats stats = values.workers[key];
                    stats.name = key;
                    worker_stats.Add(stats);
                }

                data_history.Add(current_time, worker_stats);
                previous_data_history = current_time;

                //Timer stat_timer = new Timer(
                //Thread.Sleep(60000);
            }
            else
                SaveHistory();
        }


        public void LoadHistory()
        {
            if (File.Exists("history.dat"))
            {
                this.data_history = SerializerUtilities.DeserializeHistory(File.OpenRead("history.dat"));
            }

            if (File.Exists("blocks.dat"))
            {
                this.solved_blocks = SerializerUtilities.DeserializeBlocks(File.OpenRead("blocks.dat"));
            }

            if (File.Exists("api.dat"))
            {
                this.APIKey = File.ReadAllText("api.dat");
                Window.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(Window.SetAPIBox), this.APIKey);
            }
        }

        public void SaveHistory()
        {
            if ( File.Exists("history.dat") )
                File.Delete("history.dat");

            SerializerUtilities.SerializeHistory(data_history, File.OpenWrite("history.dat"));

            if (File.Exists("blocks.dat"))
                File.Delete("blocks.dat");

            SerializerUtilities.SerializeBlocks(solved_blocks, File.OpenWrite("blocks.dat"));

            File.WriteAllText("api.dat", APIKey);
        }
    }
}
