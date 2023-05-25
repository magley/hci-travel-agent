using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using YouTravel.Util;

namespace YouTravel.Agent
{
	public partial class AgentMainWindow : Window
	{
		// TODO: Move certain properties to global settings.

		public UserConfig userConfig { get; } = UserConfig.Instance;

		public bool ToolbarVisible { get; set; } = true;

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
			OpenPage(new ArrangementList());
		}

		public void OpenPage(Page page)
		{
			myFrame.NavigationService.Navigate(page);
		}

		public void CloseMostRecentPage()
		{
			myFrame.NavigationService.RemoveBackEntry();
		}

		private void ShowToolbar_Click(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			userConfig.ToolbarVisible = menuItem.IsChecked;
			userConfig.Save();
		}

		private void On_OpenArrangementList(object sender, RoutedEventArgs e)
		{
			OpenPage(new ArrangementList());
		}

		private void On_OpenPlaceList(object sender, RoutedEventArgs e)
		{
			OpenPage(new PlacesList());
		}

		private void On_AddArrangement(object sender, RoutedEventArgs e)
		{
			OpenPage(new ArrangementAdd());
		}

		private void On_AddPlace(object sender, RoutedEventArgs e)
		{
			OpenPage(new LocationAdd());
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

		private void Next_Btn(object sender, RoutedEventArgs e)
		{
			if (myFrame.NavigationService.CanGoForward)
				myFrame.NavigationService.GoForward();
		}

		private void Back_Btn(object sender, RoutedEventArgs e)
		{
			if (myFrame.NavigationService.CanGoBack)
				myFrame.NavigationService.GoBack();
		}
	}
}
