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
			userConfig.PossibleToolbarButtons.Add(new("Add Arrangement", "IcoPlanetAdd.png", On_AddArrangement)); // For now, this dynamically changes the buttons.
			userConfig.PossibleToolbarButtons.Add(new("Add Place", "IcoLocationAdd.png", On_AddPlace));
			// ... and update this per need.
			userConfig.ToolbarButtons.Add(userConfig.PossibleToolbarButtons[0].Button);
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

		private void On_AddArrangement(object sender, RoutedEventArgs e)
		{
			userConfig.ToolbarButtons.Clear();

			foreach (var toolbarBtn in userConfig.PossibleToolbarButtons)
			{
				if (toolbarBtn.Name == "Add Place")
				{
					userConfig.ToolbarButtons.Add(toolbarBtn.Button);
				}
			}
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
