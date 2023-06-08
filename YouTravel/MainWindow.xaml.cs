using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using YouTravel.Model;

namespace YouTravel
{
    public partial class MainWindow : Window
    {
        public readonly TravelContext Ctx = new();
        private ObservableCollection<Arrangement> _arrangements = new();
        public virtual ObservableCollection<Arrangement> Arrangements { get { return _arrangements; } }

        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
            Ctx.Dispose();
        }

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitMapsApi();
            InitData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Arrangement arrangement = new()
            {
                Description = DateTime.Now.ToString()
            };

            Ctx.Arrangements.Add(arrangement);
            Ctx.SaveChanges();
        }

        #endregion

        #region Helper Funcs

        private void InitData()
        {
            Ctx.Arrangements.Load();
            _arrangements = Ctx.Arrangements.Local.ToObservableCollection();
            this.ArrangmentDataGrid.ItemsSource = _arrangements;
        }

        private void InitMapsApi()
        {
            string mapsApiKey = File.ReadAllText("Data/MapsApiKey.apikey");
            MyMap.CredentialsProvider = new ApplicationIdCredentialsProvider(mapsApiKey);
        }

        #endregion
    }
}
