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
    /// Interaction logic for ebifry.xaml
    /// </summary>
    public partial class ebifry : Window
    {
        public int w { get; private set; }
        public int h { get; private set; }
        public int x, y;

        public int eatenPoint;

        public bool live;

        public Util.rect toRect() { return new Util.rect(x, y, w, h); }
        public Point toPoint() { return new Point(x, y); }

        public ebifry()
        {
            InitializeComponent();

            Show();
            var p0 = PointToScreen(new Point(0, 0));
            var p1 = PointToScreen(new Point(Width, Height));
            w = (int)(p1.X - p0.X);
            h = (int)(p1.Y - p0.Y);
            Hide();

            live = false;
        }

        public void spawn(int ep)
        {
            live = true;
            eatenPoint = ep;
            //            Dispatcher.Invoke(() => Show());
            Show();
        }

        public void eaten()
        {
            if (eatenPoint > 0) eatenPoint--;
            else {
                live = false;
                Dispatcher.Invoke(() => Hide());
            }
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            var pt = PointToScreen(new Point(0, 0));
            x = (int)Math.Abs(pt.X);
            y = (int)Math.Abs(pt.Y);
        }
    }
}
