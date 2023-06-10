﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using YouTravel.Model;
using YouTravel.Util;

namespace YouTravel.View
{
    /// <summary>
    /// Interaction logic for ReservationsList.xaml
    /// </summary>
    public partial class TravelHistory : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public Paginator<Reservation> Paginator { get; set; } = new(10);
        public ICommand CmdFocusSearch { get; private set; }

        private DateTime _start = DateTime.MinValue;
        private DateTime _end = DateTime.MaxValue;

        public ObservableCollection<DateTime> SelectedDates { get; set; }


        private bool _isClearableCalendar = false;
        public bool IsClearableCalendar
        {
            get { return _isClearableCalendar; }
            set { _isClearableCalendar = value; DoPropertyChanged(nameof(IsClearableCalendar)); }
        }

        public TravelHistory()
        {
            InitializeComponent();
            DataContext = this;

            Paginator.PropertyChanged += SetPageNavButtonsEnabled;
            Paginator.Entities.CollectionChanged += OnReservationCollectionChanged;
            Paginator.EntitiesCurrentPage.CollectionChanged += OnReservationCurrentPageCollectionChanged;

            CmdFocusSearch = new RelayCommand(o => FocusSearch(), o => true);
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
            using (var db = new TravelContext())
            {
                db.Arrangements.Load();
                var user = YouTravelContext.User;
                Debug.Assert(user != null);
                var reservations = db.Reservations.Where(r => r.Username == user.Username);
                Paginator.Entities.Clear();
                foreach (var reservation in reservations)
                {
                    Paginator.Entities.Add(reservation);
                }
            }

            // SEARCH

            var afterSearch = Paginator.Entities
                .Where(x => searchBox.Text == "" || StringUtil.Compare(searchBox.Text, x.Arrangement.Name))
                .Where(x => x.TimeOfReservation >= _start && x.TimeOfReservation <= _end)
                .Reverse()
                .ToList();
            Paginator.Entities.Clear();
            foreach (var v in afterSearch)
            {
                Paginator.Entities.Add(v);
            }
        }

        private void OnReservationCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            LoadReservationsCurrentPage();
        }

        private void OnReservationCurrentPageCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ToggleNoPagesText();
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

        private void ViewArrangement_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var arrangement = (Arrangement)button.DataContext;
            // TODO: Navigate to view arrangement
            Console.WriteLine($"TODO: View arrangement with id {arrangement.Id}");
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LoadReservations();
            }
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Text = "";
            LoadReservations();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReservations();
        }

        private void FocusSearch()
        {
            searchBox.Focus();
            searchBox.SelectAll();
        }

        private void ArrangementCalendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (Mouse.Captured is Calendar || Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null);
            Calendar calendar = (Calendar)sender;

            try
            {
                DateTime d1 = calendar.SelectedDates[0];
                DateTime d2 = calendar.SelectedDates.Last();
                SetArrangementDates(d1, d2);
                IsClearableCalendar = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }

        private void SetArrangementDates(DateTime start, DateTime end)
        {
            if (start > end)
            {
                SetArrangementDates(end, start);
                return;
            }

            _start = start;
            _end = end;
        }

        private void ClearCalendar_Click(object sender, RoutedEventArgs e)
        {
            searchCalendar.SelectedDates.Clear();
            _start = DateTime.MinValue;
            _end = DateTime.MaxValue;
            LoadReservations();
            IsClearableCalendar = false;
        }
    }
}
