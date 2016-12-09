﻿using System;
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
        public SerifuWindow()
        {
            InitializeComponent();
        }
        public SerifuWindow(MainWindow mw): this()
        {
            this.mw = mw;
        }

        public void say(string message)
        {
            Dispatcher.Invoke(() => this.serifu.Text = message);
        }

        public void hideInvoke()
        {
            try
            {
                Dispatcher.Invoke(() => Hide());
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
        }
        public void showInvoke()
        {
            try
            {
                Dispatcher.Invoke(() => Show());
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Hide();
        }
    }
}
