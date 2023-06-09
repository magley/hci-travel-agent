﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace YouTravel.Util
{
    public class ToolbarButton
    {
        public static Button NewBtn(string imgName, RoutedEventHandler onClick, string tooltipHelp)
        {
            Button Button = new();
            Image img = new()
            {
                Source = new BitmapImage(new Uri($"pack://application:,,,/Res/{imgName}", UriKind.Absolute)),
                Width = 24,
                Height = 24
            };
            Button.Content = img;
            Button.Cursor = Cursors.Hand;
            Button.Click += onClick;

            Button.ToolTip = new Label() { Content = tooltipHelp };

            return Button;
        }
    }
}
