using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public ObservableCollection<Button> ToolbarBtn_Nav { get; set; } = new();
		public ObservableCollection<Button> ToolbarBtn_Arrangement { get; set; } = new();
		public ObservableCollection<Button> ToolbarBtn_Place { get; set; } = new();

		public AgentMainWindow()
		{
			InitializeComponent();
			DataContext = this;


			ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowLeft.png", Back_Btn));
			ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowRight.png", Next_Btn));

			ToolbarBtn_Arrangement.Add(ToolbarButton.NewBtn("IcoPlanetAdd.png", On_AddArrangement));
			ToolbarBtn_Arrangement.Add(ToolbarButton.NewBtn("IcoTravel.png", On_OpenArrangementList));

			ToolbarBtn_Place.Add(ToolbarButton.NewBtn("IcoLocationAdd.png", On_AddPlace));
			ToolbarBtn_Place.Add(ToolbarButton.NewBtn("IcoLocation.png", On_OpenPlaceList));
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
			PageBack();
			myFrame.NavigationService.RemoveBackEntry();
		}

        public void PageBack()
        {
            if (myFrame.NavigationService.CanGoBack)
                myFrame.NavigationService.GoBack();
        }

        public void PageNext()
        {
            if (myFrame.NavigationService.CanGoForward)
                myFrame.NavigationService.GoForward();
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
			PageNext();

        }

		private void Back_Btn(object sender, RoutedEventArgs e)
		{
			PageBack();

        }
	}
}
