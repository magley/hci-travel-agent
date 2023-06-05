using System.Windows;

namespace YouTravel.Shared
{
    public partial class ConfirmBox : Window
    {
        public bool Result { get; private set; } = false;
        public string MessageBody { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }

        public ConfirmBox(string messageBody, string windowTitle, string yesText, string noText)
        {
            InitializeComponent();
            DataContext = this;
            MessageBody = messageBody;
            YesText = yesText;
            NoText = noText;
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
