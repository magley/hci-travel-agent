using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Model;
using YouTravel.Shared;
using YouTravel.Util;

namespace YouTravel.View
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public UserConfig UserConfig { get; } = YouTravelContext.UserConfig;

        public ObservableCollection<Button> ToolbarBtn_Nav { get; set; } = new();
        public ObservableCollection<Button> ToolbarBtn_Arrangement { get; set; } = new();
        public ObservableCollection<Button> ToolbarBtn_Place { get; set; } = new();

        public ICommand? _cmdNavigateBack;
        public ICommand? CmdNavigateBack
        {
            get { return _cmdNavigateBack; }
            private set { _cmdNavigateBack = value; DoPropertyChanged(nameof(CmdNavigateBack)); }
        }

        public ICommand? _cmdNavigateForward;
        public ICommand? CmdNavigateForward
        {
            get { return _cmdNavigateForward; }
            private set { _cmdNavigateForward = value; DoPropertyChanged(nameof(CmdNavigateForward)); }
        }

        public ICommand? _cmdNewArrangement;
        public ICommand? CmdNewArrangement
        {
            get { return _cmdNewArrangement; }
            private set { _cmdNewArrangement = value; DoPropertyChanged(nameof(CmdNewArrangement)); }
        }

        public ICommand? _cmdViewArrangements;
        public ICommand? CmdViewArrangements
        {
            get { return _cmdViewArrangements; }
            private set { _cmdViewArrangements = value; DoPropertyChanged(nameof(CmdViewArrangements)); }
        }

        public ICommand? _cmdViewReservationsList;
        public ICommand? CmdViewReservationsList
        {
            get { return _cmdViewReservationsList; }
            private set { _cmdViewReservationsList = value; DoPropertyChanged(nameof(CmdViewReservationsList)); }
        }

        public ICommand? _cmdNewPlace;
        public ICommand? CmdNewPlace
        {
            get { return _cmdNewPlace; }
            private set { _cmdNewPlace = value; DoPropertyChanged(nameof(CmdNewPlace)); }
        }

        public ICommand? _cmdViewPlaces;
        public ICommand? CmdViewPlaces
        {
            get { return _cmdViewPlaces; }
            private set { _cmdViewPlaces = value; DoPropertyChanged(nameof(CmdViewPlaces)); }
        }

        public ICommand? _cmdViewReports;
        public ICommand? CmdViewReports
        {
            get { return _cmdViewReports; }
            private set { _cmdViewReports = value; DoPropertyChanged(nameof(CmdViewReports)); }
        }

        public ICommand? _cmdToggleToolbar;
        public ICommand? CmdToggleToolbar
        {
            get { return _cmdToggleToolbar; }
            private set { _cmdToggleToolbar = value; DoPropertyChanged(nameof(CmdToggleToolbar)); }
        }

        public ICommand? _cmdLogin;
        public ICommand? CmdLogin
        {
            get { return _cmdLogin; }
            private set { _cmdLogin = value; DoPropertyChanged(nameof(CmdLogin)); }
        }

        public ICommand? _cmdLogout;
        public ICommand? CmdLogout
        {
            get { return _cmdLogout; }
            private set { _cmdLogout = value; DoPropertyChanged(nameof(CmdLogout)); }
        }

        public ICommand? _cmdRegister;
        public ICommand? CmdRegister
        {
            get { return _cmdRegister; }
            private set { _cmdRegister = value; DoPropertyChanged(nameof(CmdRegister)); }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            InitWindowForUser();
        }

        #region InitWindowForUser

        #region InitUser
        private void InitWindowForUser()
        {
            switch (YouTravelContext.User?.Type)
            {
                case UserType.AGENT:
                    InitAgent();
                    break;
                case UserType.CLIENT:
                    InitClient();
                    break;
                default:
                    InitGuest();
                    break;
            }
        }

        private void InitAgent()
        {
            InitAgentMenu();
            InitAgentToolbar();
            InitAgentCommands();
        }

        private void InitClient()
        {
            InitClientMenu();
            InitClientToolbar();
            InitClientCommands();
        }

        private void InitGuest()
        {
            InitGuestMenu();
            InitGuestToolbar();
            InitGuestCommands();
        }
        #endregion

        #region InitMenu
        private void InitAgentMenu()
        {
            menu_item_new.Visibility = Visibility.Visible;
            menu_item_login_separator.Visibility = Visibility.Visible;

            menu_item_view_reservations_list.Visibility = Visibility.Collapsed;
            menu_item_view_places.Visibility = Visibility.Visible;
            menu_item_view_reports.Visibility = Visibility.Visible;

            menu_item_login.Visibility = Visibility.Collapsed;
            menu_item_logout.Visibility = Visibility.Visible;
            menu_item_register.Visibility = Visibility.Collapsed;
        }

        private void InitClientMenu()
        {
            menu_item_new.Visibility = Visibility.Collapsed;
            menu_item_login_separator.Visibility = Visibility.Collapsed;

            menu_item_view_reservations_list.Visibility = Visibility.Visible;
            menu_item_view_places.Visibility = Visibility.Collapsed;
            menu_item_view_reports.Visibility = Visibility.Collapsed;

            menu_item_login.Visibility = Visibility.Collapsed;
            menu_item_logout.Visibility = Visibility.Visible;
            menu_item_register.Visibility = Visibility.Collapsed;
        }

        private void InitGuestMenu()
        {
            menu_item_new.Visibility = Visibility.Collapsed;
            menu_item_login_separator.Visibility = Visibility.Collapsed;

            menu_item_view_reservations_list.Visibility = Visibility.Collapsed;
            menu_item_view_places.Visibility = Visibility.Collapsed;
            menu_item_view_reports.Visibility = Visibility.Collapsed;

            menu_item_login.Visibility = Visibility.Visible;
            menu_item_logout.Visibility = Visibility.Collapsed;
            menu_item_register.Visibility = Visibility.Visible;
        }
        #endregion

        #region InitToolbar
        private void InitAgentToolbar()
        {
            ToolbarBtn_Nav.Clear();
            ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowLeft.png", Back_Btn, "Navigate Backward (Ctrl+Left arrow)"));
            ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowRight.png", Next_Btn, "Navigate Forward (Ctrl+Right arrow)"));

            ToolbarBtn_Arrangement.Clear();
            ToolbarBtn_Arrangement.Add(ToolbarButton.NewBtn("IcoPlanetAdd.png", On_AddArrangement, "New Arrangement (Ctrl+N)"));
            ToolbarBtn_Arrangement.Add(ToolbarButton.NewBtn("IcoTravel.png", On_OpenArrangementList, "View Arrangements (Ctrl+A)"));

            ToolbarBtn_Place.Clear();
            ToolbarBtn_Place.Add(ToolbarButton.NewBtn("IcoLocationAdd.png", On_AddPlace, "New Place (Ctrl+Shift+N)"));
            ToolbarBtn_Place.Add(ToolbarButton.NewBtn("IcoLocation.png", On_OpenPlaceList, "View Places (Ctrl+P)"));
        }

        private void InitClientToolbar()
        {
            ToolbarBtn_Nav.Clear();
            ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowLeft.png", Back_Btn, "Navigate Backward (Ctrl+Left arrow)"));
            ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowRight.png", Next_Btn, "Navigate Forward (Ctrl+Right arrow)"));

            ToolbarBtn_Arrangement.Clear();
            ToolbarBtn_Arrangement.Add(ToolbarButton.NewBtn("IcoTravel.png", On_OpenArrangementList, "View available Arrangements (Ctrl+A)"));
            ToolbarBtn_Arrangement.Add(ToolbarButton.NewBtn("IcoTravelHistory.png", On_OpenReservationsList, "Travel History (Ctrl+H)"));

            ToolbarBtn_Place.Clear();
        }

        private void InitGuestToolbar()
        {
            ToolbarBtn_Nav.Clear();
            ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowLeft.png", Back_Btn, "Navigate Backward (Ctrl+Left arrow)"));
            ToolbarBtn_Nav.Add(ToolbarButton.NewBtn("IcoArrowRight.png", Next_Btn, "Navigate Forward (Ctrl+Right arrow)"));

            ToolbarBtn_Arrangement.Clear();
            ToolbarBtn_Arrangement.Add(ToolbarButton.NewBtn("IcoTravel.png", On_OpenArrangementList, "View Arrangements (Ctrl+A)"));

            ToolbarBtn_Place.Clear();
        }
        #endregion

        #region InitCommands
        private void InitAgentCommands()
        {
            CmdToggleToolbar = new RelayCommand(o => ToggleToolbar(), o => true);

            CmdNavigateBack = new RelayCommand(o => PageBack(), o => true);
            CmdNavigateForward = new RelayCommand(o => PageNext(), o => true);

            CmdNewArrangement = new RelayCommand(o => OpenPage(new ArrangementAdd(true)), o => true);
            CmdViewArrangements = new RelayCommand(o => OpenArrangementsListForUser(), o => true);
            CmdViewReservationsList = null;

            CmdNewPlace = new RelayCommand(o => OpenPage(new LocationAdd(true)), o => true);
            CmdViewPlaces = new RelayCommand(o => OpenPage(new PlacesList()), o => true);
            CmdViewReports = new RelayCommand(o => OpenPage(new MonthlyReports()), o => true);

            CmdLogin = null;
            CmdLogout = new RelayCommand(o => Logout(), o => true);
            CmdRegister = null;
        }

        private void InitClientCommands()
        {
            CmdToggleToolbar = new RelayCommand(o => ToggleToolbar(), o => true);

            CmdNavigateBack = new RelayCommand(o => PageBack(), o => true);
            CmdNavigateForward = new RelayCommand(o => PageNext(), o => true);

            CmdNewArrangement = null;
            CmdViewArrangements = new RelayCommand(o => OpenArrangementsListForUser(), o => true);
            CmdViewReservationsList = new RelayCommand(o => OpenPage(new TravelHistory()), o => true);

            CmdNewPlace = null;
            CmdViewPlaces = null;
            CmdViewReports = null;

            CmdLogin = null;
            CmdLogout = new RelayCommand(o => Logout(), o => true);
            CmdRegister = null;
        }

        private void InitGuestCommands()
        {
            CmdToggleToolbar = new RelayCommand(o => ToggleToolbar(), o => true);

            CmdNavigateBack = new RelayCommand(o => PageBack(), o => true);
            CmdNavigateForward = new RelayCommand(o => PageNext(), o => true);

            CmdNewArrangement = null;
            CmdViewArrangements = new RelayCommand(o => OpenArrangementsListForUser(), o => true);
            CmdViewReservationsList = null;

            CmdNewPlace = null;
            CmdViewPlaces = null;
            CmdViewReports = null;

            CmdLogin = new RelayCommand(o => Login(), o => true);
            CmdLogout = null;
            CmdRegister = new RelayCommand(o => Register(), o => true);
        }
        #endregion

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenArrangementsListForUser();
        }

        private void OpenArrangementsListForUser()
        {
            if (YouTravelContext.User?.Type == UserType.AGENT)
            {
                OpenPage(new ArrangementList());
            }
            else
            {
                OpenPage(new AvailableArrangements());
            }
        }

        public void SetTitle(string title)
        {
            Title = $"YouTravel - {title}";
        }

        public void OpenPage(Page page)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            myFrame.Navigate(page);
        }

        public void CloseMostRecentPage()
        {
            PageBack();
            myFrame.RemoveBackEntry();
        }

        public void PageBack()
        {
            if (myFrame.CanGoBack)
            {
                myFrame.GoBack();
            }
        }

        public void PageNext()
        {
            if (myFrame.CanGoForward)
            {
                myFrame.GoForward();
            }
        }

        private void Login()
        {
            bool? res = new Login().ShowDialog();
            if (res ?? false)
            {
                RefreshUser();
                Debug.Assert(YouTravelContext.User != null);
                //new OkBox($"Welcome {YouTravelContext.User.Username}.", "Welcome").ShowDialog();
            }
        }

        private static void Register()
        {
            var register = new Register();
            register.ShowDialog();
            var user = register.DialogResult;
            if (user != null)
            {
                new OkBox($"Successfully registered {user.Username}.", "Successful registration").ShowDialog();
            }
        }

        private void Logout()
        {
            YouTravelContext.Logout();
            RefreshUser();
        }

        private void RefreshUser()
        {
            InitWindowForUser();
            // FIXME: Doesn't clear out the last entry for some reason
            ClearNavigationHistory();
            OpenArrangementsListForUser();
        }

        private void ClearNavigationHistory()
        {
            var entry = myFrame.RemoveBackEntry();
            while (entry != null)
            {
                entry = myFrame.RemoveBackEntry();
            }
        }

        private void RefreshNavButtonEnabledStatus()
        {
            // TODO: Hardcoded everything...

            ToolbarBtn_Nav[0].IsEnabled = myFrame.CanGoBack;
            ((Image)ToolbarBtn_Nav[0].Content).Opacity = myFrame.CanGoBack ? 1 : 0.5;

            ToolbarBtn_Nav[1].IsEnabled = myFrame.CanGoForward;
            ((Image)ToolbarBtn_Nav[1].Content).Opacity = myFrame.CanGoForward ? 1 : 0.5;
        }

        private void ShowToolbar_Click(object sender, RoutedEventArgs e)
        {
            // userConfig.ToolbarVisible is changed by the binding so we don't have to.
            // Don't call ToggleToolbar() here!
            UserConfig.Save();
        }

        private void ToggleToolbar()
        {
            Btn_ToolbarShowToolbar.IsChecked ^= true;
            // This is ugly but it fixes inconsistency.
            UserConfig.Save();
        }

        private void On_OpenArrangementList(object sender, RoutedEventArgs e)
        {
            OpenArrangementsListForUser();
        }

        private void On_OpenReservationsList(object sender, RoutedEventArgs e)
        {
            OpenPage(new TravelHistory());
        }

        private void On_OpenPlaceList(object sender, RoutedEventArgs e)
        {
            OpenPage(new PlacesList());
        }

        private void On_AddArrangement(object sender, RoutedEventArgs e)
        {
            OpenPage(new ArrangementAdd(true));
        }

        private void On_AddPlace(object sender, RoutedEventArgs e)
        {
            OpenPage(new LocationAdd(true));
        }

        private void On_Login(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void On_Logout(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        private void On_Register(object sender, RoutedEventArgs e)
        {
            Register();
        }

        private void On_MonthlyReports(object sender, RoutedEventArgs e)
        {
            OpenPage(new MonthlyReports());
        }

        private void On_OpenSettings(object sender, RoutedEventArgs e)
        {
            var win = new Settings(false);
            win.ShowDialog();
        }

        private void On_OpenSettings_Toolbar(object sender, RoutedEventArgs e)
        {
            var win = new Settings(true);
            win.ShowDialog();
        }


        private void Next_Btn(object sender, RoutedEventArgs e)
        {
            PageNext();
        }

        private void Back_Btn(object sender, RoutedEventArgs e)
        {
            PageBack();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(Application.Current.Windows[0]);
            if (focusedControl is DependencyObject depObject)
            {
                string str = HelpProvider.GetHelpKey(depObject);
                HelpProvider.ShowHelp(str, this);
            }
            else
            {
                if (YouTravelContext.User?.Type == UserType.AGENT)
                {
                    HelpProvider.ShowHelp("index_agent", this);
                } else
                {
                    HelpProvider.ShowHelp("index_client", this);
                }
            }
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MyFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            RefreshNavButtonEnabledStatus();
        }

		private void On_OpenAbout(object sender, RoutedEventArgs e)
		{
            var aboutWin = new About();
            aboutWin.Owner = this;
            aboutWin.ShowDialog();
		}
	}
}
