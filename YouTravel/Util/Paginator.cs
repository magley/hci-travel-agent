using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTravel.Model;

namespace YouTravel.Util
{
    public class Paginator<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private int _pageIndex = 0;
        private int _pageCount = 0;
        public int PageIndex { get { return _pageIndex; } set { _pageIndex = value; DoPropertyChanged(nameof(PageIndex)); } }
        public int PageCount { get { return _pageCount; } set { _pageCount = value; DoPropertyChanged(nameof(PageCount)); } }
        int pageSize = 2;

        public ObservableCollection<T> Entities { get; set; } = new();
        public ObservableCollection<T> EntitiesCurrentPage { get; } = new();

        public void LoadPage()
        {
            PageCount = (int)Math.Ceiling((double)(Entities.Count / (double)pageSize));
            if (PageIndex > PageCount)
            {
                PageIndex = PageCount;
            }
            if (PageIndex <= 0 && PageCount > 0)
            {
                PageIndex = 1;
            }

            int L = Math.Max(0, (PageIndex - 1) * pageSize);
            int R = Math.Min((PageIndex) * pageSize - 1, Entities.Count - 1);

            EntitiesCurrentPage.Clear();
            if (Entities.Count != 0)
            {
                for (int i = L; i <= R; i++)
                {
                    EntitiesCurrentPage.Add(Entities[i]);
                }
            }
        }
    }
}
