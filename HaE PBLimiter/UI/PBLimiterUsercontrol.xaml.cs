using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HaE_PBLimiter;
using System.Threading;
using NLog;

namespace HaE_PBLimiter
{
    /// <summary>
    /// Interaction logic for UserControl.xaml
    /// </summary>
    public partial class PBLimiterUsercontrol : UserControl
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private PBLimiter_Logic Plugin { get; }
        private Timer timer;

        private List<PBTracker> values = new List<PBTracker>();

        public PBLimiterUsercontrol()
        {
            InitializeComponent();
        }

        public PBLimiterUsercontrol(PBLimiter_Logic plugin) : this()
        {
            Plugin = plugin;
            DataContext = plugin.Config;

            timer = new Timer(Refresh, this, 0, 1000);
        }

        private async void Refresh(object state)
        {

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                DataGrid1.ItemsSource = null;
            }));

            lock (PBData.pbPair)
            {
                values.Clear();

                foreach (var value in PBData.pbPair.Values)
                {
                    values.Add(value);
                }
            }

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                DataGrid1.ItemsSource = values;
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var overrideEditor = new OverrideEditor();
            overrideEditor.SizeToContent = SizeToContent.WidthAndHeight;
            overrideEditor.Closed += OverrideEditor_Closed;
            overrideEditor.Show();
        }

        void DataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        private void OverrideEditor_Closed(object sender, EventArgs e)
        {
            PBLimiter_Logic.Save();
        }
    }
}
