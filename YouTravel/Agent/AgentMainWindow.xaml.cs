using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Util;

namespace YouTravel.Agent
{
	public partial class AgentMainWindow : Window
	{
		public UserConfig userConfig { get; } = UserConfig.Instance;

		public ObservableCollection<Button> ToolbarBtn_Nav { get; set; } = new();
		public ObservableCollection<Button> ToolbarBtn_Arrangement { get; set; } = new();
		public ObservableCollection<Button> ToolbarBtn_Place { get; set; } = new();

		public ICommand CmdNavigateBack { get; private set; }
		public ICommand CmdNavigateForward { get; private set; }
		public ICommand CmdNewArrangement { get; private set; }
		public ICommand CmdViewArrangements { get; private set; }
		public ICommand CmdNewPlace { get; private set; }
		public ICommand CmdViewPlaces { get; private set; }

		public AgentMainWindow()
		{
			InitializeComponent();
			DataContext = this;

			ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowLeft.png", Back_Btn, "Navigate Backward (Ctrl+Left arrow)"));
			ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowRight.png", Next_Btn, "Navigate Forward (Ctrl+Right arrow)"));

			ToolbarBtn_Arrangement.Add(ToolbarButton.NewBtn("IcoPlanetAdd.png", On_AddArrangement, "New Arrangement (Ctrl+N)"));
			ToolbarBtn_Arrangement.Add(ToolbarButton.NewBtn("IcoTravel.png", On_OpenArrangementList, "View Arrangements (Ctrl+A)"));

			ToolbarBtn_Place.Add(ToolbarButton.NewBtn("IcoLocationAdd.png", On_AddPlace, "New Place (Ctrl+Shift+N)"));
			ToolbarBtn_Place.Add(ToolbarButton.NewBtn("IcoLocation.png", On_OpenPlaceList, "View Places (Ctrl+P)"));

			InitCommands();
		}

		private void InitCommands()
		{
			CmdNavigateBack = new RelayCommand(o => PageBack(), o => true);
			CmdNavigateForward = new RelayCommand(o => PageNext(), o => true);

			CmdNewArrangement = new RelayCommand(o => OpenPage(new ArrangementAdd()), o => true);
			CmdViewArrangements = new RelayCommand(o => OpenPage(new ArrangementList()), o => true);

			CmdNewPlace = new RelayCommand(o => OpenPage(new LocationAdd()), o => true);
			CmdViewPlaces = new RelayCommand(o => OpenPage(new PlacesList()), o => true);
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
			ToggleToolbar();
		}

		private void ToggleToolbar()
		{
			userConfig.ToolbarVisible = !userConfig.ToolbarVisible;
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
