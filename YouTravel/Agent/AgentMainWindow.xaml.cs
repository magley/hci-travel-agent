using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Util;

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

		public UserConfig userConfig { get; } = UserConfig.Instance;

		public bool ToolbarVisible { get; set; } = true;
		public ObservableCollection<UserControlInTab> UserControls { get; set; } = new();

		public AgentMainWindow()
		{
			InitializeComponent();
			DataContext = this;

			// The idea is to initialize this once at program startup every time...
			userConfig.PossibleToolbarButtons.Add(new("Add Arrangement", "IcoPlanetAdd.png", On_AddArrangement));
			userConfig.PossibleToolbarButtons.Add(new("Add Place", "IcoLocationAdd.png", On_AddPlace));
			userConfig.PossibleToolbarButtons.Add(new("View Arrangements", "IcoTravel.png", On_OpenArrangementList));
			userConfig.PossibleToolbarButtons.Add(new("View Places", "IcoLocation.png", On_OpenPlaceList));
			// ... and update this per need.

			userConfig.LoadToolbarConfig();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			OpenUserControl(new ArrangementList(), "Arrangements");
		}

		public void OpenUserControl(UserControl userControl, string tabName)
		{
			UserControls.Add(new(tabName, userControl));
			MyTabControl.SelectedIndex = UserControls.Count - 1;		
		}

		private void ShowToolbar_Click(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			userConfig.ToolbarVisible = menuItem.IsChecked;
			userConfig.Save();
		}

		private void TabClose_Click(object sender, RoutedEventArgs e)
		{
			// TODO: This always closes the first of its kind => Force unique tabs by Name or figure out which tab it is (use a counter instead of Name?).
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

		private void On_AddArrangement(object sender, RoutedEventArgs e)
		{
			OpenUserControl(new ArrangementAdd(), "Add Arrangement");
		}

		private void On_AddPlace(object sender, RoutedEventArgs e)
		{
			OpenUserControl(new LocationAdd(), "Add Place");
		}

		private void On_OpenSettings(object sender, RoutedEventArgs e)
		{
			var win = new Settings(false);
			win.Show();
		}

		private void On_OpenSettings_Toolbar(object sender, RoutedEventArgs e)
		{
			var win = new Settings(true);
			win.Show();
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
