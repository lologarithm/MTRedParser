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

namespace MTREDParser
{
    /// <summary>
    /// Interaction logic for StatItem.xaml
    /// </summary>
    public partial class StatItem : UserControl
    {
        public string name;
        public string mhash;
        public string rsolved;

        public StatItem(string i_name, string i_mhash, string solved)
        {
            InitializeComponent();

            this.NameBox.Text = i_name;
            this.MhashBox.Text = i_mhash;
            this.SolvedBox.Text = solved;
        }
    }
}
