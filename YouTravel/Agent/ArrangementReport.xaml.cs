using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Model;
using YouTravel.Util;

namespace YouTravel.Agent
{
    public partial class ArrangementReport : Page
    {
        public Paginator<Reservation> Paginator = new();

        public Arrangement Arrangement { get; set; }

        private Reservation? _selectedReservation = null;

        public ArrangementReport(Arrangement arrangement)
        {
            InitializeComponent();
            DataContext = this;

            Paginator.PropertyChanged += SetPageNavButtonsEnabled;
            Paginator.Entities.CollectionChanged += OnReservationCollectionChanged;
            Paginator.EntitiesCurrentPage.CollectionChanged += OnReservationCurrentPageCollectionChanged;

            using (var db = new TravelContext())
            {
                // NOTE: This is an issue with lazy loading: you have to explicitly
                // tell the context to fetch the other entity, too.
                Arrangement = db.Arrangements.Include(a => a.Reservations).Where(a => a.Id == arrangement.Id).First();
            }
            foreach (var reservation in Arrangement.Reservations)
            {
                Paginator.Entities.Add(reservation);
            }
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
            ReselectReservation();
        }

        private void ReselectReservation()
        {
            if (_selectedReservation != null)
            {
                int index = -1;
                for (int i = 0; i < Paginator.EntitiesCurrentPage.Count; i++)
                {
                    if (Paginator.EntitiesCurrentPage[i].Id == _selectedReservation.Id)
                    {
                        index = i;
                    }
                }

                if (index > -1)
                {
                    tbReservations.SelectedIndex = index;
                }
            }

            if (Paginator.EntitiesCurrentPage.Count > 0 && _selectedReservation == null)
            {
                tbReservations.SelectedIndex = 0;
            }
        }

        private void SetPageNavButtonsEnabled(object? sender, PropertyChangedEventArgs e)
        {
            btnPrevPage.IsEnabled = Paginator.PageIndex > 1;
            btnNextPage.IsEnabled = Paginator.PageIndex < Paginator.PageCount;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			Mouse.OverrideCursor = null;
		}

        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            Paginator.PageIndex--;
            LoadReservationsCurrentPage();
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Paginator.PageIndex++;
            LoadReservationsCurrentPage();
        }
    }
}
