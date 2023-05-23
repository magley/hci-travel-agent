using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace YouTravel.Util
{
    public class ToolbarButton
    {
		public string Name { get; }
		public Button Button { get; }

        public ToolbarButton(string name, string filename, RoutedEventHandler onClick) {
			Name = name;
			Button = new Button();
			Image img = new()
			{
				Source = new BitmapImage(new Uri($"pack://application:,,,/Res/{filename}", UriKind.Absolute)),
				Width = 24,
				Height = 24
			};
			Button.Content = img;
			Button.Click += onClick;
		}
    }
}
