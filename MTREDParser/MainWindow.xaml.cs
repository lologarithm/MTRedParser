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
            StartQuery.IsEnabled = false;
            LoadData.IsEnabled = false;
            StopButton.IsEnabled = true;

            if (APIKeyBox.Text == "")
                APIKeyBox.Text = "2aab78a9691c0e2f2ce7425957de20dc";

            if (cs == null)
            {
                cs = new CalculateStats(this);
            }

            t = new Thread(cs.DoStats);
            t.Start(APIKeyBox.Text);
        }

        public void EmptyListBox()
        {
            StatsBox.Items.Clear();
        }

        public void FillListBox(List<MTREDWorkerStats> stats_list)
        {
            foreach ( MTREDWorkerStats worker_stats in stats_list)
            {
                WorkerTotalControl worker_control = new WorkerTotalControl(worker_stats.name, worker_stats.rsolved);
                StatsBox.Items.Add(worker_control);
            }
        }

        public void AddListBox(MTREDWorkerStats worker_stats)
        {
            WorkerTotalControl worker_control = new WorkerTotalControl(worker_stats.name, worker_stats.rsolved);
            StatsBox.Items.Add(worker_control);
        }

        public void SetAPIBox(string value)
        {
            APIKeyBox.Text = value;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (cs != null)
                cs.run = false;
            if ( t != null)
                t.Join();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StatsBox.Height = ((Grid)sender).ActualHeight - 50;
            StatsBox.Width = ((Grid)sender).ActualWidth;
        }

        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            LoadData.IsEnabled = false;

            if ( cs == null )
            cs = new CalculateStats(this);

            Thread load_thread = new Thread(cs.LoadHistory);
            load_thread.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = false;
            
            if (cs != null)
                cs.run = false;
            if (t != null)
            {
                t.Join();
            }

            StartQuery.IsEnabled = true;
        }
    }
}
