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
using System.Windows.Shapes;

namespace seyana
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private SeyanaBrain brain;
        private ConfigWindow()
        {
            InitializeComponent();
        }
        public ConfigWindow(SeyanaBrain brain): this()
        {
            this.brain = brain;
            for (int i = 0; i < scaleList.Count; ++i) if (SeyanaBrain.scale == scaleList[i]) cScale.SelectedIndex = i;
            for (int i = 0; i < speedList.Count; ++i) if (brain.speed == speedList[i]) cSpeed.SelectedIndex = i;
        }

        public enum ConfigEvent { OKEVENT, CANCELEVENT }
        private List<double> scaleList = new List<double> { 0.35, 0.7, 1 };
        private List<double> speedList = new List<double> { 20, 8, 4 };

        public double scale { private set; get; }
        public double speed { private set; get; }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (cScale.SelectedIndex >= 0 && cScale.SelectedIndex < scaleList.Count) scale = scaleList[cScale.SelectedIndex];
            if (cSpeed.SelectedIndex >= 0 && cSpeed.SelectedIndex < speedList.Count) speed = speedList[cSpeed.SelectedIndex];
            brain.closeConfig(ConfigEvent.OKEVENT, this);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            brain.closeConfig(ConfigEvent.CANCELEVENT, this);
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            brain.closeConfig(ConfigEvent.CANCELEVENT, this);
        }
    }
}
