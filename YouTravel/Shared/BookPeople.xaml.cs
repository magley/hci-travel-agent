using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YouTravel.Model;
using static YouTravel.Shared.ConfirmBox;

namespace YouTravel.Shared
{
    /// <summary>
    /// Interaction logic for BookPeople.xaml
    /// </summary>
    public partial class BookPeople : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        private string? _result = null;
        public string? Result
        {
            get { return _result; }
            set { _result = value; DoPropertyChanged(nameof(Result)); }
        }
        public string MessageBody { get; set; }
        public string? YesText { get; set; }
        public string? NoText { get; set; }

        private bool _formValid = false;
        public bool FormValid
        {
            get { return _formValid; }
            set { _formValid = value; DoPropertyChanged(nameof(FormValid)); }
        }

        private bool _inputError = true;
        public bool InputError
        {
            get { return _inputError; }
            set { _inputError = value; DoPropertyChanged(nameof(InputError)); }
        }

        public BookPeople(string messageBody, string windowTitle, string? yesText, string? noText)
        {
            InitializeComponent();
            DataContext = this;
            MessageBody = messageBody;
            Title = windowTitle;
            YesText = yesText;
            NoText = noText;

            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Owner = mainWindow;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = null;
            Close();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (Result == null)
            {
                InputError = true;
                FormValid = false;
                return;
            }
            try
            {
                var res = int.Parse(Result);
                if (res < 0) throw new Exception();
                InputError = false;
                FormValid = true;
            }
            catch (Exception ex)
            {
                InputError = true;
                FormValid = false;
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
