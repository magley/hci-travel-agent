using System.IO;
using System;
using System.Windows;
using System.Windows.Input;
using YouTravel.Agent;

namespace YouTravel.Shared
{
	public partial class HelpViewer : Window
	{
		public HelpViewer(string key, Window originator)
		{
			InitializeComponent();

			string currentDirectory = Directory.GetCurrentDirectory();
			string path = $"{currentDirectory}/Help/{key}.html";

			if (!File.Exists(path))
			{
				key = "error";
			}
			path = $"{currentDirectory}/Help/{key}.html";

			Uri uri = new($"file:///{path}");
			wbHelp.Navigate(uri);
		}
		private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (wbHelp != null) && wbHelp.CanGoBack;
		}

		private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			wbHelp.GoBack();
		}

		private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (wbHelp != null) && wbHelp.CanGoForward;
		}

		private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			wbHelp.GoForward();
		}

		private void wbHelp_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
		}
	}
}
