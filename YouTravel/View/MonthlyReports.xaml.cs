using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Model;
using YouTravel.Util;

namespace YouTravel.View
{
    /// <summary>
    /// Interaction logic for MonthlyReports.xaml
    /// </summary>
    public partial class MonthlyReports : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                DoPropertyChanged(nameof(SelectedDate));
                LoadReservations();
            }
        }

        public Paginator<Reservation> Paginator { get; set; } = new(10);
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
            TitleOverride.PageNameAsWords(this);
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
            ctx.Arrangements.Load();

            Paginator.Entities.Clear();
            foreach (var v in ctx.Reservations.Local)
            {
                Paginator.Entities.Add(v);
            }

            var rangeStart = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
            var rangeEnd = rangeStart.AddMonths(1).AddDays(-1);

            var inThisMonth = Paginator.Entities
                .Where(x => x.TimeOfReservation >= rangeStart && x.TimeOfReservation <= rangeEnd)
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

        private void BtnPrevMonth_Click(object sender, RoutedEventArgs e)
        {
            SelectedDate = SelectedDate.AddMonths(-1);
            SetMonthButtonsEnabled();
        }

        private void BtnNextMonth_Click(object sender, RoutedEventArgs e)
        {
            SelectedDate = SelectedDate.AddMonths(1);
            SetMonthButtonsEnabled();
        }

        private void SelectedDate_Click(object sender, RoutedEventArgs e)
        {
            SelectedDate = DateTime.Now;
        }

        private void SetMonthButtonsEnabled()
        {
            var maxYear = DateTime.MaxValue.Year;
            var maxMonth = DateTime.MaxValue.Month;

            var minYear = DateTime.MinValue.Year;
            var minMonth = DateTime.MinValue.Month;

            var curYear = SelectedDate.Year;
            var curMonth = SelectedDate.Month;

            btnPrevMonth.IsEnabled = curYear != minYear || curMonth > minMonth;
            btnNextMonth.IsEnabled = curYear != maxYear || curMonth < maxMonth;
        }

        private void ViewArrangement_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Navigate to view arrangement
            Console.WriteLine("TODO: View arrangement");
        }
    }
}
