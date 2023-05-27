using System.Windows;

namespace YouTravel.Shared
{
    public partial class ConfirmBox : Window
    {
        public bool Result { get; private set; } = false;
        public string MessageBody { get; set; }

        public ConfirmBox(string messageBody, string windowTitle)
        {
            InitializeComponent();
            DataContext = this;
            MessageBody = messageBody;
            Title = windowTitle;

            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Owner = mainWindow;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }
    }
}
