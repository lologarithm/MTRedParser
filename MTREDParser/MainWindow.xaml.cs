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
using MTREDParser.BitFloor;

namespace MTREDParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<MTREDWorkerStats> StoredStats = new List<MTREDWorkerStats>();
        private Button query = null;
        private Dictionary<Rectangle, BidInfo> rect_bid_info = new Dictionary<Rectangle, BidInfo>();
        Thread t = null;
        CalculateStats cs = null;

        public MainWindow()
        {
            InitializeComponent();
            query = QueryStats;
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

        private void QueryStats_Click(object sender, RoutedEventArgs e)
        {
            BitFloorGrid.Children.Clear();
            BitFloorGrid.Children.Add(query);

            var json = new WebClient().DownloadString("https://api.bitfloor.com/book/L2/1");
            BitFloorStats values = JsonConvert.DeserializeObject<BitFloorStats>(json);
            double max_size = 0;
            int total_count = 0;
            foreach (string[] key_values in values.asks)
            {
                total_count++;
                double val = double.Parse(key_values[1]);
                if ( val > max_size )
                    max_size = val;
            }

            foreach (string[] key_values in values.bids)
            {
                total_count++;
                double val = double.Parse(key_values[1]);
                if (val > max_size)
                    max_size = val;
            }

            int count = 0;
            int num_col = values.asks.Length + values.bids.Length + 1;
            double col_width = (BitFloorGrid.ActualWidth - 50) / (num_col);
            
            for ( int i = values.bids.Length - 1; i >= 0; i-- )
            {
                string[] key_values = values.bids[i];

                Rectangle rect = new Rectangle();
                rect.Width = col_width;
                rect.Height = Math.Min(Math.Max((double.Parse(key_values[1]) / max_size) * (BitFloorGrid.ActualHeight - 100), 10), BitFloorGrid.ActualHeight - 100);
                rect.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                rect.Margin = new Thickness((35 + (col_width * count) + (col_width / num_col) * count), 0, 0, 100);
                rect.Fill = new SolidColorBrush(Colors.Red);
                rect.Opacity = .7;
                rect.MouseEnter += new MouseEventHandler(rect_MouseEnter);
                rect.MouseLeave += new MouseEventHandler(rect_MouseLeave);

                BidInfo bi = CreateBidInfo(double.Parse(key_values[0]), double.Parse(key_values[1]));
                rect_bid_info.Add(rect, bi);
                BitFloorGrid.Children.Add(rect);
                if (count == 0)
                {
                    TextBlock bot_vert_text = new TextBlock();
                    bot_vert_text.Text = Math.Round( max_size , (int)2).ToString();
                    bot_vert_text.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    bot_vert_text.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    bot_vert_text.Margin = new Thickness(1, 0, 0, BitFloorGrid.ActualHeight - 25); 
                    bot_vert_text.Height = 25;
                    BitFloorGrid.Children.Add(bot_vert_text);

                    TextBlock bot_hor_text = new TextBlock();
                    bot_hor_text.Text = Math.Round(decimal.Parse(key_values[0]), (int)2).ToString();
                    bot_hor_text.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    bot_hor_text.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    bot_hor_text.Margin = new Thickness(20, 0, 0, 70);
                    bot_hor_text.Height = 25;
                    BitFloorGrid.Children.Add(bot_hor_text);
                }

                count++;
            }

            foreach (string[] key_values in values.asks)
            {
                Rectangle rect = new Rectangle();
                rect.Width = col_width;
                rect.Height = Math.Min(Math.Max((double.Parse(key_values[1]) / max_size) * (BitFloorGrid.ActualHeight - 100), 10), BitFloorGrid.ActualHeight - 100);
                rect.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                rect.Margin = new Thickness((35 + (col_width * count) + (col_width / num_col) * count), 0, 0, 100);
                rect.Fill = new SolidColorBrush(Colors.Blue);
                rect.Opacity = .7;
                rect.MouseEnter += new MouseEventHandler(rect_MouseEnter);
                rect.MouseLeave += new MouseEventHandler(rect_MouseLeave);
                BidInfo bi = CreateBidInfo(double.Parse(key_values[0]), double.Parse(key_values[1]));
                rect_bid_info.Add(rect, bi);
                BitFloorGrid.Children.Add(rect);

                if (count == total_count - 1)
                {
                    TextBlock top_vert_text = new TextBlock();
                    top_vert_text.Text = "0";
                    top_vert_text.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    top_vert_text.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    top_vert_text.Margin = new Thickness(1, 0, 0, 80);
                    top_vert_text.Height = 25;
                    BitFloorGrid.Children.Add(top_vert_text);

                    TextBlock top_hor_text = new TextBlock();
                    top_hor_text.Text = Math.Round(decimal.Parse(key_values[0]), (int)2).ToString();
                    top_hor_text.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    top_hor_text.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    top_hor_text.Margin = new Thickness(BitFloorGrid.ActualWidth - 50, 0, 0, 70);
                    top_hor_text.Height = 25;
                    BitFloorGrid.Children.Add(top_hor_text);
                }
                
                
                count++;


            }

            Line hor_line = new Line();
            hor_line.X1 = 10;
            hor_line.Y1 = 10;
            hor_line.X2 = BitFloorGrid.ActualWidth - 35;
            hor_line.Y2 = 10;
            hor_line.StrokeThickness = 2;
            hor_line.Stroke = new SolidColorBrush(Colors.Black);
            hor_line.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            hor_line.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            hor_line.Margin = new Thickness(24, 0, 0, 99);


            Line vert_line = new Line();
            vert_line.X1 = 10;
            vert_line.Y1 = BitFloorGrid.ActualHeight - 101;
            vert_line.X2 = 10;
            vert_line.Y2 = 0;
            vert_line.StrokeThickness = 2;
            vert_line.Stroke = new SolidColorBrush(Colors.Black);
            vert_line.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            vert_line.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            vert_line.Margin = new Thickness(24, 0, 0, 99);

            BitFloorGrid.Children.Add(hor_line);
            BitFloorGrid.Children.Add(vert_line);

            var tick_json = new WebClient().DownloadString("https://api.bitfloor.com/ticker/1");
            Tick tick_values = JsonConvert.DeserializeObject<Tick>(tick_json);
            BidInfo last_bid = CreateBidInfo(tick_values.price, tick_values.size);
            last_bid.Margin = new Thickness(BitFloorGrid.ActualWidth / 2, 0, 0, 0);
            last_bid.bidgrid.Background = new SolidColorBrush(Colors.LightGreen);
            BitFloorGrid.Children.Add(last_bid);
            
        }

        void rect_MouseLeave(object sender, MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.Opacity = .7;
            BitFloorGrid.Children.Remove(((BidInfo)rect_bid_info[rect]));
        }

        void rect_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            rect.Opacity = 1.0;
            BitFloorGrid.Children.Add(((BidInfo)rect_bid_info[rect]));

        }


        private BidInfo CreateBidInfo(double value, double amount)
        {
            BidInfo bi = new BidInfo();
            bi.Amount.Text = Math.Round(amount, 3).ToString();
            bi.Value.Text = Math.Round(value, 3).ToString();
            bi.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            bi.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            return bi;
        }
    }
}
