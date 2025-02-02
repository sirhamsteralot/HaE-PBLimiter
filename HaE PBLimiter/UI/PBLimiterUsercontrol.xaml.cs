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

        private Dictionary<long, PBTracker>[] values = new Dictionary<long, PBTracker>[2];
        private int resourceInUse = 0;


        public PBLimiterUsercontrol()
        {
            values[0] = new Dictionary<long, PBTracker>();
            values[1] = new Dictionary<long, PBTracker>();
            InitializeComponent();

            PBData.OnUpdate += PBData_OnUpdate;
        }

        private void PBData_OnUpdate(PBTracker obj, long id)
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                if (resourceInUse == 0)
                    values[1][id] = obj;
                else
                    values[0][id] = obj;
            }));
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

                if (resourceInUse == 0)
                    resourceInUse = 1;
                else
                    resourceInUse = 0;

                DataGrid1.ItemsSource = values[resourceInUse].Values;
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
