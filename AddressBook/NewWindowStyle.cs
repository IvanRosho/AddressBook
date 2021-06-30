using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Collections;

namespace AddressBook
{
    public partial class NewWindowStyle
    {

        public void CloseClick(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
        }

        public void MinimizeClick(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        public void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (App.Current.MainWindow.WindowState==WindowState.Normal)
            App.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                App.Current.MainWindow.WindowState = WindowState.Normal;
        }
        void TitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                App.Current.MainWindow.DragMove();
            }
        }

        void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var wind = App.Current.MainWindow;
                if (wind.WindowState == WindowState.Maximized)
                {
                    wind.BeginInit();
                    double adjustment = 40.0;
                    var mouse1 = e.MouseDevice.GetPosition(wind);
                    var width1 = Math.Max(wind.ActualWidth - 2 * adjustment, adjustment);
                    wind.WindowState = WindowState.Normal;
                    var width2 = Math.Max(wind.ActualWidth - 2 * adjustment, adjustment);
                    wind.Left = (mouse1.X - adjustment) * (1 - width2 / width1);
                    wind.Top = -7;
                    wind.EndInit();
                    wind.DragMove();
                }
            }
        }
    }
}
