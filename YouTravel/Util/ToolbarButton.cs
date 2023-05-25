using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace YouTravel.Util
{
    public class ToolbarButton
    {
		public static Button NewBtn(string imgName, RoutedEventHandler onClick)
		{
			Button Button = new();
			Image img = new()
			{
				Source = new BitmapImage(new Uri($"pack://application:,,,/Res/{imgName}", UriKind.Absolute)),
				Width = 24,
				Height = 24
			};
			Button.Content = img;
			Button.Click += onClick;

			return Button;
		}
    }
}
