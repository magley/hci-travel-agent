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
        public Paginator<Reservation> Paginator { get; set; } = new(10);

        public Arrangement Arrangement { get; set; }

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
            ToggleNoPagesText();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((AgentMainWindow)Window.GetWindow(this)).SetTitle(TitleRegex.PageNameAsWords(this));
            Mouse.OverrideCursor = null;
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
