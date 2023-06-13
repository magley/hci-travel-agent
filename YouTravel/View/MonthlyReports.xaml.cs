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

        private enum ColumnType
        {
            ID,
            RESERVED_BY,
            NAME,
            RESERVED_ON,
            PEOPLE,
            PAID
        }
        private ColumnType _sortColumnType = ColumnType.ID;
        private bool _sortAscending = true;

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

            // SORT

            inThisMonth.Sort((a, b) =>
            {
                switch (_sortColumnType)
                {
                    case ColumnType.ID: default: return a.Id.CompareTo(b.Id);
                    case ColumnType.RESERVED_BY: return a.Username.CompareTo(b.Username);
                    case ColumnType.RESERVED_ON:return a.TimeOfReservation.CompareTo(b.TimeOfReservation);
                    case ColumnType.NAME: return a.Arrangement.Name.CompareTo(b.Arrangement.Name);
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
                inThisMonth.Reverse();
            }

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
            var button = (Button)sender;
            var arrangement = (Arrangement)button.DataContext;
            ((MainWindow)Window.GetWindow(this)).OpenPage(new ArrangementEdit(arrangement));
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
                case "Name":
                    _sortColumnType = ColumnType.NAME;
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

            LoadReservations();
            e.Handled = true;
        }
    }
}
