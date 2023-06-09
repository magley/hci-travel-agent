using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Model;
using YouTravel.Shared;
using YouTravel.Util;

namespace YouTravel.View
{
    public partial class ArrangementList : Page
    {
        public Paginator<Arrangement> Paginator { get; set; } = new();

        public bool ShowActive { get; set; } = true;
        public bool ShowFinished { get; set; } = false;
        public bool ShowUpcoming { get; set; } = true;

        public ICommand CmdFocusSearch { get; private set; }

        private Arrangement? _selectedArrangement = null;

        public ArrangementList()
        {
            InitializeComponent();
            DataContext = this;
            Paginator.PropertyChanged += SetPageNavButtonsEnabled;
            Paginator.Entities.CollectionChanged += OnArrangementCollectionChanged;
            Paginator.EntitiesCurrentPage.CollectionChanged += OnArrangementVisibleCollectionChanged;

            CmdFocusSearch = new RelayCommand(o => FocusSearch(), o => true);
        }

        private void FocusSearch()
        {
            this.searchBox.Focus();
            this.searchBox.SelectAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleOverride.PageNameAsWords(this);
            InitDbContext();
            Mouse.OverrideCursor = null;
        }

        private void InitDbContext()
        {
            LoadArrangements();
        }

        private void LoadArrangements()
        {
            using var ctx = new TravelContext();
            ctx.Arrangements.Load();

            Paginator.Entities.Clear();
            foreach (var v in ctx.Arrangements.Local)
            {
                Paginator.Entities.Add(v);
            }

            // SEARCH

            var afterSearch = Paginator.Entities
                .Where(x => searchBox.Text == "" || StringUtil.Compare(searchBox.Text, x.Name))
                .Where(x => (ShowActive && x.IsActive()) ||
                            (ShowFinished && x.IsFinished()) ||
                            (ShowUpcoming && x.IsUpcoming())
                )
                .Reverse()
                .ToList();
            Paginator.Entities.Clear();
            foreach (var v in afterSearch)
            {
                Paginator.Entities.Add(v);
            }
        }

        private void LoadArrangementsPage()
        {
            Paginator.LoadPage();

            arrangementsList.DataContext = Paginator.EntitiesCurrentPage;
            ReselectArrangement();
        }

        private void ReselectArrangement()
        {
            if (_selectedArrangement != null)
            {
                int index = -1;
                for (int i = 0; i < Paginator.EntitiesCurrentPage.Count; i++)
                {
                    if (Paginator.EntitiesCurrentPage[i].Id == _selectedArrangement.Id)
                    {
                        index = i;
                    }
                }

                if (index > -1)
                {
                    arrangementsList.SelectedIndex = index;
                }
            }
        }

        private void OnArrangementVisibleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ToggleNoArrangementsText();
        }

        private void OnArrangementCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            LoadArrangementsPage();
        }

        private void ToggleNoArrangementsText()
        {
            if (Paginator.EntitiesCurrentPage.Count > 0)
            {
                txtNoArrangements.Visibility = Visibility.Collapsed;
                arrangementsList.Visibility = Visibility.Visible;
            }
            else
            {
                txtNoArrangements.Visibility = Visibility.Visible;
                arrangementsList.Visibility = Visibility.Collapsed;
            }
        }

        private void EditArrangement_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Arrangement arr = (Arrangement)button.DataContext;

            ((MainWindow)Window.GetWindow(this)).OpenPage(new ArrangementEdit(arr));
        }

        private void ArrangementReport_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Arrangement arr = (Arrangement)button.DataContext;

            ((MainWindow)Window.GetWindow(this)).OpenPage(new ArrangementReport(arr));
        }

        private void RemoveArrangement_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = false;
            ConfirmBox confirmBox = new("Are you sure you want to delete this arrangement?", "Delete confirmation", "Delete", "Cancel", ConfirmBox.ConfirmBoxIcon.QUESTION);
            if (confirmBox.ShowDialog() == false)
            {
                confirmed = confirmBox.Result;
            }
            if (!confirmed)
            {
                return;
            }

            using var ctx = new TravelContext();
            Button btn = (Button)sender;
            int arrId = ((Arrangement)btn.DataContext).Id;

            Arrangement? arr = ctx.Arrangements.Find(arrId);
            if (arr == null)
            {
                Console.WriteLine("I can't do it.");
                return;
            }

            ctx.Arrangements.Remove(arr);
            ctx.SaveChanges();

            SoundUtil.PlaySound("snd_delete.wav");
            LoadArrangements();
        }

        private void CbShowActive_Click(object sender, RoutedEventArgs e)
        {
            LoadArrangements();
        }

        private void CbShowFinished_Click(object sender, RoutedEventArgs e)
        {
            LoadArrangements();
        }

        private void CbShowUpcoming_Click(object sender, RoutedEventArgs e)
        {
            LoadArrangements();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadArrangements();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Text = "";
            LoadArrangements();
        }

        private void BtnNewArrangement_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).OpenPage(new ArrangementAdd(true));
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            Paginator.PageIndex--;
            LoadArrangementsPage();
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            Paginator.PageIndex++;
            LoadArrangementsPage();
        }

        private void SetPageNavButtonsEnabled(object? sender, PropertyChangedEventArgs e)
        {
            btnPrevPage.IsEnabled = Paginator.PageIndex > 1;
            btnNextPage.IsEnabled = Paginator.PageIndex < Paginator.PageCount;
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LoadArrangements();
            }
        }

        private void ArrangementsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _selectedArrangement = Paginator.EntitiesCurrentPage[arrangementsList.SelectedIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }
    }
}
