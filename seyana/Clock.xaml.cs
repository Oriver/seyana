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
        private SeyanaBrain brain;
        public Clock()
        {
            InitializeComponent();
            w = (int)Width;
            Task.Factory.StartNew(run);
            brain = SeyanaBrain.SeyanaBrainFactory;
        }

        private enum context { CLOCK, TIMER};
        private context nowContext = context.CLOCK;

        private DateTime endTime;

        public void setTimer(int h, int m, int s)
        {
            var now = DateTime.Now;
            endTime = now.AddHours(h).AddMinutes(m).AddSeconds(s);
            nowContext = context.TIMER;
        }

        private bool endFlg = false;
        public void run()
        {
            while (true)
            {
                if (endFlg) return;

                var dt = DateTime.Now;
                switch (nowContext)
                {
                    case context.CLOCK:
                        {
                            Dispatcher.Invoke(() => l.Content = dt.ToString("HH:mm"));
                            break;
                        }
                    case context.TIMER:
                        {
                            var last = endTime - dt;
                            if (last.TotalMilliseconds < 0)
                            {
                                brain.endTimer();
                                nowContext = context.CLOCK;
                            }
                            else {
                                Dispatcher.Invoke(() => l.Content = last.ToString(@"hh\:mm\:ss"));
                            }
                            break;
                        }
                    default:
                        break;
                }

                System.Threading.Thread.Sleep(1000 / SeyanaBrain.FPS);
            }
        }

        public void end()
        {
            endFlg = true;
            Close();
        }
    }
}
