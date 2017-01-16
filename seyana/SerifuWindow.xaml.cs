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
    /// Interaction logic for serifuWindow.xaml
    /// </summary>
    public partial class SerifuWindow : Window
    {
        MainWindow mw;
        SeyanaBrain brain;
        public int w { private set; get; }
        public int h { private set; get; }
        public SerifuWindow()
        {
            InitializeComponent();
            w = (int)Width;
            h = (int)Height;
            brain = SeyanaBrain.SeyanaBrainFactory;
        }
        public SerifuWindow(MainWindow mw): this()
        {
            this.mw = mw;
        }

        public void say(string message)
        {
            Dispatcher.Invoke(() => this.serifu.Text = message);
        }
        public void hide()
        {
            if(IsVisible) Dispatcher.Invoke(() => Hide());
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Hide();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            brain.keyPressed(e);
        }
    }
}
