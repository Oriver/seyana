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
    /// Interaction logic for Clock.xaml
    /// </summary>
    public partial class Clock : Window
    {
        public int w { private set; get; }
        public Clock()
        {
            InitializeComponent();
            w = (int)Width;
            Task.Factory.StartNew(run);
        }

        private bool endFlg = false;
        public void run()
        {
            while (true)
            {
                if (endFlg) return;

                var dt = DateTime.Now;
                Dispatcher.Invoke(() => l.Content = dt.ToString("HH:mm"));

                System.Threading.Thread.Sleep(1000 / SeyanaBrain.FPS);
            }
        }

        public void end()
        {
            Console.WriteLine("check");
            endFlg = true;
            Close();
        }
    }
}
