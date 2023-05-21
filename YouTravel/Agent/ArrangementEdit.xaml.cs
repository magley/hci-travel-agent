using System.Windows;
using YouTravel.Model;

namespace YouTravel.Agent
{
    public partial class ArrangementEdit : Window
    {
        private Arrangement arrangement;
        public ArrangementEdit(Arrangement arrangement)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
            DataContext = this;

            this.arrangement = arrangement;
        }
    }
}
