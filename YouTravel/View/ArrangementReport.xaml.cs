using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
    public partial class ArrangementReport : Page
    {
        public Paginator<Reservation> Paginator { get; set; } = new(10);

        public Arrangement Arrangement { get; set; }

        private enum ColumnType
        {
            ID,
            RESERVED_BY,
            RESERVED_ON,
            PEOPLE,
            PAID
        }
        private ColumnType _sortColumnType = ColumnType.ID;
        private bool _sortAscending = true;

        public ArrangementReport(Arrangement arrangement)
        {
            InitializeComponent();
            DataContext = this;

            Paginator.PropertyChanged += SetPageNavButtonsEnabled;
            Paginator.Entities.CollectionChanged += OnReservationCollectionChanged;
            Paginator.EntitiesCurrentPage.CollectionChanged += OnReservationCurrentPageCollectionChanged;

            using var ctx = new TravelContext();
            Arrangement = ctx.Arrangements.Include(a => a.Reservations).Where(a => a.Id == arrangement.Id).First();

            LoadData();
        }

        private void LoadData()
        {
            var data = Arrangement.Reservations.ToList();

            // SORT

            data.Sort((a, b) =>
            {
                switch (_sortColumnType)
                {
                    case ColumnType.ID: default: return a.Id.CompareTo(b.Id);
                    case ColumnType.RESERVED_BY: return a.Username.CompareTo(b.Username);
                    case ColumnType.RESERVED_ON: return a.TimeOfReservation.CompareTo(b.TimeOfReservation);
                    case ColumnType.PEOPLE: return a.NumOfPeople.CompareTo(b.NumOfPeople);
                    case ColumnType.PAID:
                        {
                            if (b.PaidOn == null)
                            {
                                return -1;
                            }
                            else
                            {
                                if (a.PaidOn == null)
                                {
                                    return 1;
                                }
                                var d1 = (DateTime)a.PaidOn;
                                var d2 = (DateTime)b.PaidOn;
                                return d1.CompareTo(d2);
                            }
                        }
                }
            });
            if (_sortAscending == false)
            {
                data.Reverse();
            }

            Paginator.Entities.Clear();
            foreach (var reservation in data)
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
            TitleOverride.PageNameAsWords(this);
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

        private void tbReservations_Sorting(object sender, DataGridSortingEventArgs e)
        {
            int colIndex = e.Column.DisplayIndex; // Affected by reordering columns, no way to know the absolute index.
            string headerName = (string)tbReservations.ColumnFromDisplayIndex(colIndex).Header;
            switch (headerName)
            {
                case "Id":
                    _sortColumnType = ColumnType.ID;
                    break;
                case "Reserved by":
                    _sortColumnType = ColumnType.RESERVED_BY;
                    break;
                case "Reserved on":
                    _sortColumnType = ColumnType.RESERVED_ON;
                    break;
                case "People":
                    _sortColumnType = ColumnType.PEOPLE;
                    break;
                case "Paid?":
                    _sortColumnType = ColumnType.PAID;
                    break;
                default:
                    Console.WriteLine($"WARNING: Unknown column for sorting: {headerName}");
                    _sortColumnType = (ColumnType)colIndex;
                    break;
            }

            _sortAscending ^= true;

            LoadData();
            e.Handled = true;
        }
    }
}
