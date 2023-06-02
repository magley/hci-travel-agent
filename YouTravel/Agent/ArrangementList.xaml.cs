using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YouTravel.Model;
using YouTravel.Shared;
using YouTravel.Util;

namespace YouTravel.Agent
{
    public partial class ArrangementList : Page, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler? PropertyChanged;
		void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

		public ObservableCollection<Arrangement> Arrangements { get; } = new();
		public ObservableCollection<Arrangement> ArrangementsCurrentPage { get; } = new();

        private int _pageIndex = 0;
        private int _pageCount = 0;
        public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; DoPropertyChanged(nameof(PageIndex)); SetPageNavButtonsEnabled(); } }
		public int PageCount { get { return _pageCount; } set { _pageCount = value; DoPropertyChanged(nameof(PageCount)); SetPageNavButtonsEnabled(); } }
        int pageSize = 2;

		public bool ShowActive { get; set; } = true;
        public bool ShowFinished { get; set; } = false;
        public bool ShowUpcoming { get; set; } = true;

		public ArrangementList()
        {
            InitializeComponent();
            DataContext = this;
            Arrangements.CollectionChanged += OnArrangementCollectionChanged;
			ArrangementsCurrentPage.CollectionChanged += OnArrangementVisibleCollectionChanged;
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitDbContext();
        }

        private void InitDbContext()
        {
            LoadArrangements();
        }

        private void LoadArrangements()
        {
            using (var ctx = new TravelContext())
            {
                ctx.Arrangements.Load();

                Arrangements.Clear();
                foreach (var v in ctx.Arrangements.Local)
                {
                    Arrangements.Add(v);
                }

				// SEARCH

				var afterSearch = Arrangements
					.Where(x => searchBox.Text == "" || StringUtil.Compare(searchBox.Text, x.Name))
					.Where(x => (ShowActive && x.IsActive()) ||
								(ShowFinished && x.IsFinished()) ||
								(ShowUpcoming && x.IsUpcoming())
					)
					.ToList();
				Arrangements.Clear();
				foreach (var v in afterSearch)
				{
					Arrangements.Add(v);
				}   
            }
		}

		private void LoadArrangementsPage()
        {
			PageCount = (int)Math.Ceiling((double)(Arrangements.Count / (double)pageSize));
			if (PageIndex > PageCount)
			{
				PageIndex = PageCount;
			}
            if (PageIndex <= 0 && PageCount > 0)
            {
                PageIndex = 1;
            }

			int L = Math.Max(0, (PageIndex-1) * pageSize);
            int R = Math.Min((PageIndex) * pageSize - 1, Arrangements.Count - 1);

			ArrangementsCurrentPage.Clear();
            if (Arrangements.Count != 0)
            {
				for (int i = L; i <= R; i++)
                {
                    ArrangementsCurrentPage.Add(Arrangements[i]);
				}
			}

			arrangementsList.DataContext = ArrangementsCurrentPage;
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
            if (ArrangementsCurrentPage.Count > 0)
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

            ((AgentMainWindow)Window.GetWindow(this)).OpenPage(new ArrangementEdit(arr));
        }

        private void ArrangementReport_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Arrangement arr = (Arrangement)button.DataContext;

			((AgentMainWindow)Window.GetWindow(this)).OpenPage(new ArrangementReport(arr));
        }

        private void RemoveArrangement_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = false;
            ConfirmBox confirmBox = new("Are you sure you want to delete this arrangement?", "Delete Confirmation");
            if (confirmBox.ShowDialog() == false)
            {
                confirmed = confirmBox.Result;
            }
            if (!confirmed)
            {
                return;
            }

            using (var ctx = new TravelContext())
            {
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

                LoadArrangements();
            }
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
            string searchQuery = this.searchBox.Text;
            LoadArrangements();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Text = "";
            LoadArrangements();
		}

		private void BtnNewArrangement_Click(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;

			((AgentMainWindow)Window.GetWindow(this)).OpenPage(new ArrangementAdd());
		}

		private void btnPrevPage_Click(object sender, RoutedEventArgs e)
		{
			PageIndex--;
			LoadArrangementsPage();
		}

		private void btnNextPage_Click(object sender, RoutedEventArgs e)
		{
			PageIndex++;
			LoadArrangementsPage();
		}

        private void SetPageNavButtonsEnabled()
        {
			btnPrevPage.IsEnabled = PageIndex > 1;
			btnNextPage.IsEnabled = PageIndex < PageCount;
		}
	}
}
