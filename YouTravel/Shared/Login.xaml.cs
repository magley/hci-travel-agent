using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YouTravel.Model;
using YouTravel.Util;

namespace YouTravel.Shared
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private string _username = "";
        public string Username
        {
            get { return _username; }
            set { _username = value; DoPropertyChanged(nameof(Username)); }
        }

        private string _password = "";
        public string Password
        {
            get { return _password; }
            set { _password = value; DoPropertyChanged(nameof(Password)); }
        }

        private string? _errorText;
        public string? ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; DoPropertyChanged(nameof(ErrorText)); }
        }

        public Login()
        {
            InitializeComponent();
            DataContext = this;

            Owner = Application.Current.MainWindow;
            tbUsername.Focus();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }

        private void AttemptLogin()
        {
            try
            {
                YouTravelContext.Login(Username, Password);
                DialogResult = true;
                Close();
            }
            catch (LoginFailedException)
            {
                ErrorText = "Wrong username or password";
            }
        }

        private void TbUsername_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptLogin();
            }
            else
            {
                ErrorText = null;
            }
        }

        private void TbPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AttemptLogin();
            }
            else
            {
                ErrorText = null;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void TbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = ((PasswordBox)sender).Password;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(Application.Current.Windows[1]);
            if (focusedControl is DependencyObject depObject)
            {
                string str = HelpProvider.GetHelpKey(depObject);
                HelpProvider.ShowHelp(str, this);
            }
            else
            {
                if (YouTravelContext.User?.Type == UserType.AGENT)
                {
                    HelpProvider.ShowHelp("index_agent", this);
                }
                else
                {
                    HelpProvider.ShowHelp("index_client", this);
                }
            }
        }
    }
}
