using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using YouTravel.Model;

namespace YouTravel.Shared
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private string _username = "";
        public string Username
        {
            get { return _username; }
            set { _username = value; DoPropertyChanged(nameof(Username)); }
        }

        private string _name = "";
        // Can't use Name since it hides an inherited member
        public string UName
        {
            get { return _name; }
            set { _name = value; DoPropertyChanged(nameof(UName)); }
        }

        private string _surname = "";
        public string Surname
        {
            get { return _surname; }
            set { _surname = value; DoPropertyChanged(nameof(Surname)); }
        }

        private string _email = "";
        public string Email
        {
            get { return _email; }
            set { _email = value; DoPropertyChanged(nameof(Email)); }
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

        public new User? DialogResult { get; set; }

        public Register()
        {
            InitializeComponent();
            DataContext = this;

            // TODO: Is this necessary if we call the window with ShowDialog() instead of Show()?
            Owner = Application.Current.MainWindow;
            tbUsername.Focus();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            AttemptRegister();
        }

        private void AttemptRegister()
        {
            using var db = new TravelContext();
            var user = new User()
            {
                Username = Username,
                Name = UName,
                Surname = Surname,
                Email = Email,
                Password = Password,
            };
            db.Users.Add(user);
            db.SaveChanges();
            DialogResult = user;
            Close();
        }

        private void TbUsername_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUpHelper(e.Key);
        }

        private void TbName_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUpHelper(e.Key);
        }

        private void TbSurname_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUpHelper(e.Key);
        }

        private void TbEmail_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUpHelper(e.Key);
        }

        private void TbPassword_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUpHelper(e.Key);
        }

        private void KeyUpHelper(Key key)
        {
            if (key == Key.Enter)
            {
                AttemptRegister();
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
    }
}
