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

namespace HaE_PBLimiter.UI
{
    /// <summary>
    /// Interaction logic for UserControl.xaml
    /// </summary>
    public partial class PBLimiterUsercontrol : UserControl
    {
        private PBLimiter_Logic Plugin { get; }

        public PBLimiterUsercontrol()
        {
            InitializeComponent();
        }

        public PBLimiterUsercontrol(PBLimiter_Logic plugin) : this()
        {
            Plugin = plugin;
            DataContext = plugin.Config;
        }
    }
}
