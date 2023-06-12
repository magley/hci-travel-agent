using System.Diagnostics;
using System.Media;
using System.Windows;

namespace YouTravel.Shared
{
	public partial class About : Window
	{
		public About()
		{
			InitializeComponent();
			SystemSounds.Beep.Play();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			var url = e.Uri.ToString();
			Process.Start(new ProcessStartInfo(url)
			{
				UseShellExecute = true
			});
		}
	}
}
