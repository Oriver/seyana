using System;
using System.Collections.Generic;
using System.IO;
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
            for (int i = 0; i < scaleList.Count; ++i) if (brain.scale == scaleList[i]) cScale.SelectedIndex = i;
            for (int i = 0; i < speedList.Count; ++i) if (brain.speed == speedList[i]) cSpeed.SelectedIndex = i;
            for (int i = 0; i < rwtList.Count; ++i) if (brain.randomWalkThreshold == rwtList[i]) cRWT.SelectedIndex = i;
        }
        public ConfigWindow(double scale, double speed, double randomWalkThreshold): this()
        {
            brain = SeyanaBrain.SeyanaBrainFactory;
            this.scale = scale;
            this.speed = speed;
            this.randomWalkThreshold = randomWalkThreshold;
            for (int i = 0; i < scaleList.Count; ++i) if (scale == scaleList[i]) cScale.SelectedIndex = i;
            for (int i = 0; i < speedList.Count; ++i) if (speed == speedList[i]) cSpeed.SelectedIndex = i;
            for (int i = 0; i < rwtList.Count; ++i) if (randomWalkThreshold == rwtList[i]) cRWT.SelectedIndex = i;
        }

        public configContainer toConfigContainer()
        {
            return new configContainer(scale, speed, randomWalkThreshold);
        }
        public static explicit operator configContainer(ConfigWindow cw)
        {
            return cw.toConfigContainer();
        }

        public enum ConfigEvent { OKEVENT, CANCELEVENT }
        private List<double> scaleList = new List<double> { 0.35, 0.7, 1 };
        private List<double> speedList = new List<double> { 20, 8, 4 };
        private List<double> rwtList = new List<double> { 0, 0.005, 0.02, 0.1 };

        public double scale { private set; get; }
        public double speed { private set; get; }
        public double randomWalkThreshold { private set; get; }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (cScale.SelectedIndex >= 0 && cScale.SelectedIndex < scaleList.Count) scale = scaleList[cScale.SelectedIndex];
            if (cSpeed.SelectedIndex >= 0 && cSpeed.SelectedIndex < speedList.Count) speed = speedList[cSpeed.SelectedIndex];
            if (cRWT.SelectedIndex >= 0 && cRWT.SelectedIndex < rwtList.Count) randomWalkThreshold = rwtList[cRWT.SelectedIndex];
            brain.closeConfig(ConfigEvent.OKEVENT, toConfigContainer());
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            brain.closeConfig(ConfigEvent.CANCELEVENT, toConfigContainer());
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            brain.closeConfig(ConfigEvent.CANCELEVENT, toConfigContainer());
        }
    }

    public class configContainer
    {
        public double scale, speed, randomWalkThreshold;

        private const string savefile = "config.dat";

        public configContainer()
        {
            var brain = SeyanaBrain.SeyanaBrainFactory;
            scale = brain.scale;
            speed = brain.speed;
            randomWalkThreshold = brain.randomWalkThreshold;
        }
        public configContainer(double scale, double speed, double randomWalkThreshold)
        {
            this.scale = scale;
            this.speed = speed;
            this.randomWalkThreshold = randomWalkThreshold;
        }

        public ConfigWindow toConfigWindow()
        {
            return new ConfigWindow(scale, speed, randomWalkThreshold);
        }
        public static explicit operator ConfigWindow(configContainer cc)
        {
            return cc.toConfigWindow();
        }

        public void save()
        {
            try
            {
                var w = new BinaryWriter(new FileStream(savefile, FileMode.Create));

                w.Write(scale);
                w.Write(speed);
                w.Write(randomWalkThreshold);

                w.Close();
            }catch(Exception) { }
        }

        public static configContainer load()
        {
            try
            {
                if (!File.Exists(savefile)) return new configContainer(1, 8, 0.02);

                var r = new BinaryReader(new FileStream(savefile, FileMode.Open));
                var buf = new byte[sizeof(double) * 3];
                double scale, speed, randomWalkThreshold;

                r.Read(buf, 0, sizeof(double) * 3);
                scale = BitConverter.ToDouble(buf, 0);
                speed = BitConverter.ToDouble(buf, sizeof(double));
                randomWalkThreshold = BitConverter.ToDouble(buf, sizeof(double) * 2);

                r.Close();
                return new configContainer(scale, speed, randomWalkThreshold);
            }catch(Exception) { }
            return new configContainer();
        }
    }
}
