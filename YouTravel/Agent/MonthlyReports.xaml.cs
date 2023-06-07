using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YouTravel.Model;
using YouTravel.Util;

namespace YouTravel.Agent
{
    /// <summary>
    /// Interaction logic for MonthlyReports.xaml
    /// </summary>
    public partial class MonthlyReports : Page
    {
        public List<string> Months { get; } = new()
        {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
        };
        private int _selectedMonthIndex = DateTime.Now.Month;
        public int SelectedMonthIndex
        {
            get { return _selectedMonthIndex; }
            set
            {
                _selectedMonthIndex = value;
                LoadReservations();
            }
        }

        public Paginator<Reservation> Paginator = new();
        public MonthlyReports()
        {
            InitializeComponent();
            DataContext = this;

            Paginator.PropertyChanged += SetPageNavButtonsEnabled;
            Paginator.Entities.CollectionChanged += OnReservationCollectionChanged;
            Paginator.EntitiesCurrentPage.CollectionChanged += OnReservationCurrentPageCollectionChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((AgentMainWindow)Window.GetWindow(this)).SetTitle(TitleRegex.PageNameAsWords(this));
            InitDbContext();
            Mouse.OverrideCursor = null;
        }

        private void InitDbContext()
        {
            LoadReservations();
        }

        private void LoadReservations()
        {
            using var ctx = new TravelContext();
            ctx.Reservations.Load();

            Paginator.Entities.Clear();
            foreach (var v in ctx.Reservations.Local)
            {
                Paginator.Entities.Add(v);
            }

            int year = DateTime.Now.Year;
            var firstDayOfMonth = new DateTime(year, SelectedMonthIndex, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var inThisMonth = Paginator.Entities
                .Where(x => x.TimeOfReservation >= firstDayOfMonth && x.TimeOfReservation <= lastDayOfMonth)
                .ToList();
            Paginator.Entities.Clear();
            foreach (var v in inThisMonth)
            {
                Paginator.Entities.Add(v);
            }
        }

        private void ToggleNoPagesText()
        {
            if (Paginator.Entities.Count > 0)
            {
                txtNoReservations.Visibility = Visibility.Collapsed;
                tbReservations.Visibility = Visibility.Visible;
            }
            else
            {
                txtNoReservations.Visibility = Visibility.Visible;
                tbReservations.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadReservationsCurrentPage()
        {
            Paginator.LoadPage();
            tbReservations.DataContext = Paginator.EntitiesCurrentPage;
        }

        private void OnReservationCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            LoadReservationsCurrentPage();
        }

        private void OnReservationCurrentPageCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ToggleNoPagesText();
        }

        private void SetPageNavButtonsEnabled(object? sender, PropertyChangedEventArgs e)
        {
            btnPrevPage.IsEnabled = Paginator.PageIndex > 1;
            btnNextPage.IsEnabled = Paginator.PageIndex < Paginator.PageCount;
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            Paginator.PageIndex--;
            LoadReservationsCurrentPage();
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Paginator.PageIndex++;
            LoadReservationsCurrentPage();
        }
    }
}
