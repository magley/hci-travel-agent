using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace YouTravel.Agent
{
	public class UserControlInTab
	{
		public UserControl? UserControl { get; set; } = null;
		public string Name { get; set; } = "";

		public UserControlInTab(string name, UserControl userControl)
		{
			Name = name;
			UserControl = userControl;
		}

		public UserControlInTab() { }
	}

	public partial class AgentMainWindow : Window
	{
		// TODO: Move certain properties to global settings.

		public bool ToolbarVisible { get; set; } = true;
		public ObservableCollection<UserControlInTab> UserControls { get; set; } = new();

		public AgentMainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			UserControls.Add(new("Arrangements", new ArrangementList()));
			UserControls.Add(new("Places", new PlacesList()));

			MyTabControl.SelectedIndex = 0;
		}

		private void ShowToolbar_Click(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			ToolbarVisible = menuItem.IsChecked;
			toolbarTray.Visibility = ToolbarVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		private void TabClose_Click(object sender, RoutedEventArgs e)
		{
			string s = (string)((Button)sender).CommandParameter;
			var uc = UserControls.Where(uc => uc.Name == s).SingleOrDefault();
			if (uc != null)
			{
				UserControls.Remove(uc);
			}
		}
	}
}
