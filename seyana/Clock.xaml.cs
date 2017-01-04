using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private bool isTimer;

        public void setTimer(int h, int m, int s)
        {
            var now = DateTime.Now;
            endTime = now.AddHours(h).AddMinutes(m).AddSeconds(s);
            nowContext = context.TIMER;
            isTimer = true;
        }

        private bool endFlg = false;
        public void run()
        {
            while (true)
            {
                if (endFlg) return;

                var dt = DateTime.Now;
                var last = endTime - dt;
                if (last.TotalSeconds < 0 && isTimer)
                {
                    brain.endTimer();
                    isTimer = false;
                }

                switch (nowContext)
                {
                    case context.CLOCK:
                        {
                            Dispatcher.Invoke(() => l.Content = dt.ToString("HH:mm"));
                            break;
                        }
                    case context.TIMER:
                        {
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

        private void timerStop()
        {
            var last = endTime - DateTime.Now;
            if (last.TotalSeconds > 0)
            {
                if (nowContext == context.CLOCK) nowContext = context.TIMER;
                else nowContext = context.CLOCK;
            }
            else
            {
                brain.timerClicked();
            }
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            timerStop();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            timerStop();
        }

        private void Timer_Click(object sender, RoutedEventArgs e)
        {
            if (l.Visibility == Visibility.Visible)
            {
                l.Visibility = Visibility.Hidden;
                tbox.Visibility = Visibility.Visible;
                tbox.SelectAll();
            }else
            {
                l.Visibility = Visibility.Visible;
                tbox.Visibility = Visibility.Hidden;
            }
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            try {
                if (l.Visibility == Visibility.Hidden && e.Key == Key.Enter)
                {
                    // 空白は潰す
                    string s = tbox.Text.Replace(" ", "");

                    // hh:mm:ssのパターン
                    var mc1 = new Regex(@"\d?\d:\d?\d:\d?\d");
                    // mm:ssのパターン
                    var mc2 = new Regex(@"\d?\d:\d?\d");
                    // ssのパターン
                    var mc3 = new Regex(@"\d+");
                    // hh hのパターン
                    var mc4 = new Regex(@"\d+[hH]");
                    // mm mのパターン
                    var mc5 = new Regex(@"\d+[mM]");
                    // ss sのパターン
                    var mc6 = new Regex(@"\d+[sS]");

                    if (mc1.IsMatch(s))
                    {
                        var split = mc1.Match(s).Value.Split(':');
                        setTimer(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
                        l.Visibility = Visibility.Visible;
                        tbox.Visibility = Visibility.Hidden;
                    } else if (mc2.IsMatch(s))
                    {
                        var split = mc2.Match(s).Value.Split(':');
                        setTimer(0, int.Parse(split[0]), int.Parse(split[1]));
                        l.Visibility = Visibility.Visible;
                        tbox.Visibility = Visibility.Hidden;
                    }
                    else if (mc4.IsMatch(s) || mc5.IsMatch(s) || mc6.IsMatch(s))
                    {
                        int h = 0, m = 0, sec = 0;
                        if (mc4.IsMatch(s)) h = int.Parse(mc4.Match(s).Value.Substring(0, mc4.Match(s).Value.Length - 1));
                        if (mc5.IsMatch(s)) m = int.Parse(mc5.Match(s).Value.Substring(0, mc5.Match(s).Value.Length - 1));
                        if (mc6.IsMatch(s)) sec = int.Parse(mc6.Match(s).Value.Substring(0, mc6.Match(s).Value.Length - 1));

                        setTimer(h, m, sec);
                        l.Visibility = Visibility.Visible;
                        tbox.Visibility = Visibility.Hidden;
                    }
                    else if (mc3.IsMatch(s))
                    {
                        setTimer(0, 0, int.Parse(mc3.Match(s).Value));
                        l.Visibility = Visibility.Visible;
                        tbox.Visibility = Visibility.Hidden;
                    }
                }
            }catch(Exception) { }
        }
    }
}
