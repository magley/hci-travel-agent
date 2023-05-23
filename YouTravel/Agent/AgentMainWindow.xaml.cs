using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

	public static class Command
	{
		public static readonly RoutedUICommand CmdCloseCurrentTab = new RoutedUICommand("Do something", "DoSomething", typeof(AgentMainWindow));
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

		public void OpenUserControl(UserControl userControl, string tabName)
		{
			UserControls.Add(new(tabName, userControl));
			MyTabControl.SelectedIndex = UserControls.Count - 1;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			OpenUserControl(new ArrangementList(), "Arrangements");
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
			var uc = UserControls.Where(uc => uc.Name == s).FirstOrDefault();
			if (uc != null)
			{
				UserControls.Remove(uc);
			}
		}

		private void On_OpenArrangementList(object sender, RoutedEventArgs e)
		{
			OpenUserControl(new ArrangementList(), "Arrangements");
		}

		private void On_OpenPlaceList(object sender, RoutedEventArgs e)
		{
			OpenUserControl(new PlacesList(), "Places");
		}

		private void On_AddPlace(object sender, RoutedEventArgs e)
		{
			OpenUserControl(new LocationAdd(), "Add Place");
		}

		private void CmdCloseCurrentTab_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			int currentTab = MyTabControl.SelectedIndex;
			if (currentTab != -1)
			{
				UserControls.RemoveAt(currentTab);
			}
		}

		private void CmdCloseCurrentTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
	}
}
