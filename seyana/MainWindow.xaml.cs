﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace seyana
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SeyanaBrain brain;
        SerifuWindow sw;
        ebifry ebi;

        public static int x { get; private set; }
        public static int y { get; private set; }
        private const int WIDTH = 300, HEIGHT = 300;
        private static int width, height;
        public int w
        {
            get
            {
                return (int)(width * brain.scale);
            }
            private set { width = value; }
        }
        public int h
        {
            get
            {
                return (int)(height * brain.scale);
            }
            private set { height = value; }
        }
        public Point toPoint() { return new Point(x, y); }
        public Util.rect toRect() { return new Util.rect(x, y, w, h); }

        public MainWindow()
        {
            InitializeComponent();

            Show();

            brain = SeyanaBrain.SeyanaBrainFactory;
            sw = new SerifuWindow();
            ebi = new ebifry();

            brain.init(this, sw, ebi);
            sw.Owner = this;
            ebi.Owner = this;

            x = 100;
            y = 200;

            Width = WIDTH;
            Height = HEIGHT;
            var p0 = PointToScreen(new Point(0, 0));
            var p1 = PointToScreen(new Point(Width, Height));
            w = (int)(p1.X - p0.X);
            h = (int)(p1.Y - p0.Y);

            setPosition();
        }

        private static bool moving = false;
        private void setPosition()
        {
            setPosition(this, x, y);
        }
        private void setPosition(int x, int y)
        {
            setPosition(this, x, y);
        }
        private void setPosition(UIElement e, int x, int y)
        {
            try
            {
                var p0 = PointToScreen(new Point(0, 0));
                var p1 = PointFromScreen(new Point(p0.X - x, p0.Y - y));
                Canvas.SetLeft(e, Math.Abs(p1.X));
                Canvas.SetTop(e, Math.Abs(p1.Y));
                if (e == this)
                {
                    var p2 = PointToScreen(new Point(0, 0));
                    MainWindow.x = (int)p2.X;
                    MainWindow.y = (int)p2.Y;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
        }
        public void setPositionInvoke(UIElement e, int x, int y)
        {
            if (!moving)
            {
                moving = true;
                Dispatcher.Invoke(() => {
                    setPosition(e, x, y);
                    moving = false;
                });
            }
        }

        /// <summary>
        /// speak something
        /// </summary>
        /// <param name="message">something to say</param>
        public void say(string message)
        {
            sw.say(message);
            Dispatcher.Invoke(() => sw.Show());
        }

        /// <summary>
        /// summon ebifry
        /// </summary>
        public void createEbi()
        {
            // 前のがまだ生きてたら召喚しない
            if (ebi.live) return;

            do
            {
                double ang = Util.rnd.NextDouble() * 2 * Math.PI;
                double x0 = (x + w / 2) + 100 * Math.Cos(ang) - ebi.w / 2;
                double y0 = (y + h / 2) + 100 * Math.Sin(ang) - ebi.h / 2;
                ebi.x = (int)x0;
                ebi.y = (int)y0;
                setPosition(ebi, (int)x0, (int)y0);
            } while (!Util.isInScreen(ebi.toRect()));
            ebi.spawn((int)(100 / brain.scale / brain.scale));
        }

        public void setScale()
        {
            Dispatcher.Invoke(() =>
            {
                Width = WIDTH * brain.scale;
                Height = HEIGHT * brain.scale;
                invert.ScaleX = brain.scale * (invert.ScaleX < 0 ? -1 : 1);
                invert.ScaleY = brain.scale * (invert.ScaleY < 0 ? -1 : 1);
            });
            
        }

        public static bool looking = false;
        public static int faceTo = 1; // 1 = left, -1 = right
        public void faceLeft()
        {
            if (faceTo == -1)
            {
                Dispatcher.Invoke(() => invert.ScaleX = Math.Abs(invert.ScaleX));
                faceTo = 1;
            }
        }
        public void faceRight()
        {
            if (faceTo == 1)
            {
                Dispatcher.Invoke(() => invert.ScaleX = -Math.Abs(invert.ScaleX));
                faceTo = -1;
            }
        }
        public void faceToCursor()
        {
            if(!looking)
            {
                looking = true;
                Dispatcher.Invoke(() =>
                {
                    var p = System.Windows.Forms.Cursor.Position;
                    var wp = new Point(p.X, p.Y);
                    var src = PresentationSource.FromVisual(this);
                    var mp = src.CompositionTarget.TransformFromDevice.Transform(wp);
                    if (mp.X < x) faceLeft();
                    else faceRight();
                    looking = false;
                });
            }
        }

        /// <summary>
        /// action something(to test)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Action_Clicked(object sender, RoutedEventArgs args)
        {
            createEbi();
        }

        /// <summary>
        /// menu: summon ebifry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Summon_Clicked(object sender, RoutedEventArgs args)
        {
            createEbi();
        }

        public void Config_Clicked(object sender, RoutedEventArgs args)
        {
            brain.openConfig();
        }

        /// <summary>
        /// menu: Quit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Quit_Clicked(object sender, RoutedEventArgs args)
        {
            brain.close();
            Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            brain.clicked();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            brain.keyPressed(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            sw.Close();
            ebi.Close();
        }
    }
}
