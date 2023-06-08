using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Util;

namespace YouTravel.Agent
{
    public partial class AgentMainWindow : Window
    {
        public UserConfig UserConfig { get; } = UserConfig.Instance;

        public ObservableCollection<Button> ToolbarBtn_Nav { get; set; } = new();
        public ObservableCollection<Button> ToolbarBtn_Arrangement { get; set; } = new();
        public ObservableCollection<Button> ToolbarBtn_Place { get; set; } = new();

        public ICommand CmdNavigateBack { get; private set; }
        public ICommand CmdNavigateForward { get; private set; }
        public ICommand CmdNewArrangement { get; private set; }
        public ICommand CmdViewArrangements { get; private set; }
        public ICommand CmdNewPlace { get; private set; }
        public ICommand CmdViewPlaces { get; private set; }
        public ICommand CmdViewReports { get; private set; }
        public ICommand CmdToggleToolbar { get; private set; }

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

            #region InitCommands
            CmdToggleToolbar = new RelayCommand(o => ToggleToolbar(), o => true);

            CmdNavigateBack = new RelayCommand(o => PageBack(), o => true);
            CmdNavigateForward = new RelayCommand(o => PageNext(), o => true);

            CmdNewArrangement = new RelayCommand(o => OpenPage(new ArrangementAdd(true)), o => true);
            CmdViewArrangements = new RelayCommand(o => OpenPage(new ArrangementList()), o => true);

            CmdNewPlace = new RelayCommand(o => OpenPage(new LocationAdd(true)), o => true);
            CmdViewPlaces = new RelayCommand(o => OpenPage(new PlacesList()), o => true);
            CmdViewReports = new RelayCommand(o => OpenPage(new MonthlyReports()), o => true);
            #endregion
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenPage(new ArrangementList());
        }

        public void SetTitle(string title)
        {
            Title = $"YouTravel - {title}";
        }

        public void OpenPage(Page page)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
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
            {
                myFrame.NavigationService.GoBack();
            }
        }

        public void PageNext()
        {
            if (myFrame.NavigationService.CanGoForward)
            {
                myFrame.NavigationService.GoForward();
            }
        }

        private void RefreshNavButtonEnabledStatus()
        {
            // TODO: Hardcoded everything...

            ToolbarBtn_Nav[0].IsEnabled = myFrame.NavigationService.CanGoBack;
            ((Image)ToolbarBtn_Nav[0].Content).Opacity = myFrame.CanGoBack ? 1 : 0.5;

            ToolbarBtn_Nav[1].IsEnabled = myFrame.NavigationService.CanGoForward;
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
            OpenPage(new ArrangementList());
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

        private void On_MonthlyReports(object sender, RoutedEventArgs e)
        {
            OpenPage(new MonthlyReports());
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

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(Application.Current.Windows[0]);
            if (focusedControl is DependencyObject)
            {
                string str = HelpProvider.GetHelpKey((DependencyObject)focusedControl);
                HelpProvider.ShowHelp(str, this);
            }
            //else
            //{
            //             HelpProvider.ShowHelp("index", this);
            //         }
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void myFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            RefreshNavButtonEnabledStatus();
        }
    }
}
