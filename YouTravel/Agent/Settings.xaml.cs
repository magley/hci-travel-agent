using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Util;

namespace YouTravel.Agent
{

    public partial class Settings : Window, INotifyPropertyChanged
    {
        private bool _ToolbarVisible;
        private double _Latitude;
        private double _Longitude;
        private bool _ToolbarShowNav;
        private bool _ToolbarShowArrangement;
        private bool _ToolbarShowPlace;

        public bool ToolbarVisible { get { return _ToolbarVisible; } set { _ToolbarVisible = value; DoPropertyChanged(nameof(ToolbarVisible)); } }
        public double Latitude { get { return _Latitude; } set { _Latitude = value; DoPropertyChanged(nameof(Latitude)); } }
        public double Longitude { get { return _Longitude; } set { _Longitude = value; DoPropertyChanged(nameof(Longitude)); } }
        public bool ToolbarShowNav { get { return _ToolbarShowNav; } set { _ToolbarShowNav = value; DoPropertyChanged(nameof(ToolbarShowNav)); } }
        public bool ToolbarShowArrangement { get { return _ToolbarShowArrangement; } set { _ToolbarShowArrangement = value; DoPropertyChanged(nameof(ToolbarShowArrangement)); } }
        public bool ToolbarShowPlace { get { return _ToolbarShowPlace; } set { _ToolbarShowPlace = value; DoPropertyChanged(nameof(ToolbarShowPlace)); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public UserConfig UserConfig { get; } = YouTravelContext.UserConfig;

        private string? _selectedSection;
        public string? SelectedSection { get { return _selectedSection; } set { _selectedSection = value; DoPropertyChanged(nameof(SelectedSection)); } }

        public Settings(bool openToolbarSectionOfSettings)
        {
            InitializeComponent();

            if (openToolbarSectionOfSettings)
            {
                TreeViewItem_Toolbar.IsSelected = true;
            }
            else
            {
                TreeViewItem_General.IsSelected = true;
            }
            DataContext = this;
            Owner = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToolbarVisible = UserConfig.ToolbarVisible;
            Latitude = UserConfig.StartLocation_Lat;
            Longitude = UserConfig.StartLocation_Long;
            ToolbarShowNav = UserConfig.ToolbarNav_Visible;
            ToolbarShowArrangement = UserConfig.ToolbarArrangement_Visible;
            ToolbarShowPlace = UserConfig.ToolbarPlace_Visible;
            Mouse.OverrideCursor = null;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var newVal = ((TreeViewItem)e.NewValue).Header.ToString();
            SelectedSection = newVal;
        }

        private void BtnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            UserConfig.ToolbarVisible = ToolbarVisible;
            UserConfig.StartLocation_Lat = Latitude;
            UserConfig.StartLocation_Long = Longitude;
            UserConfig.ToolbarNav_Visible = ToolbarShowNav;
            UserConfig.ToolbarArrangement_Visible = ToolbarShowArrangement;
            UserConfig.ToolbarPlace_Visible = ToolbarShowPlace;

            UserConfig.Save();
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
