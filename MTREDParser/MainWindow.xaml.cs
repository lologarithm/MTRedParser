using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using Newtonsoft.Json;
using System.Threading;

namespace MTREDParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<MTREDWorkerStats> StoredStats = new List<MTREDWorkerStats>();
        Thread t = null;
        CalculateStats cs = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Test()
        {

        }

        private void StartQuery_Click(object sender, RoutedEventArgs e)
        {
            if (APIKeyBox.Text == "")
                APIKeyBox.Text = "2aab78a9691c0e2f2ce7425957de20dc";

            cs = new CalculateStats(this);

            t = new Thread(cs.DoStats);
            t.Start(APIKeyBox.Text);
        }

        public void AddListItem(List<MTREDWorkerStats> stats_list)
        {
            //StatsBox.Items.Clear();

            foreach (MTREDWorkerStats stats in stats_list)
            {
                StatItem si = new StatItem(stats.name, stats.mhash.ToString(), stats.rsolved.ToString());
                StatsBox.Items.Add(si);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (cs != null)
                cs.run = false;
            if ( t != null)
                t.Join();
        }
    }
}
