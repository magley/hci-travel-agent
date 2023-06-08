using System.Media;
using System.Windows;

namespace YouTravel.Shared
{
    public partial class ConfirmBox : Window
    {
        public bool Result { get; private set; } = false;
        public string MessageBody { get; set; }
        public string? YesText { get; set; }
        public string? NoText { get; set; }
        private readonly ConfirmBoxIcon icon;

        public enum ConfirmBoxIcon
        {
            QUESTION,
            INFO
        }

        public ConfirmBox(string messageBody, string windowTitle, string? yesText, string? noText, ConfirmBoxIcon icon)
        {
            InitializeComponent();
            DataContext = this;
            MessageBody = messageBody;

            this.icon = icon;

            if (yesText == null)
            {
                btnYes.Visibility = Visibility.Collapsed;
            }
            else
            {
                YesText = yesText;

            }

            if (noText == null)
            {
                btnNo.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoText = noText;
            }

            Title = windowTitle;

            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Owner = mainWindow;

            SetIcon();
        }

        private void SetIcon()
        {
            imgIconInfo.Visibility = Visibility.Collapsed;
            imgIcoQuestion.Visibility = Visibility.Collapsed;

            switch (icon)
            {
                case ConfirmBoxIcon.INFO:
                    SystemSounds.Beep.Play();
                    imgIconInfo.Visibility = Visibility.Visible;
                    break;
                case ConfirmBoxIcon.QUESTION:
                    SystemSounds.Asterisk.Play();
                    imgIcoQuestion.Visibility = Visibility.Visible;
                    break;
                default:
                    return;
            }
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
