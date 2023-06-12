using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace YouTravel.Util
{
    // Singleton.
    public class UserConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void DoPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private string _LoggedInUserUsername = "";
        private bool _ToolbarVisible = true;
        private bool _ToolbarNav_Visible = true;
        private bool _ToolbarArrangement_Visible = true;
        private bool _ToolbarPlace_Visible = true;
        private double _StartLocation_Lat = 45.2;
        private double _StartLocation_Long = 19;

        public string LoggedInUserUsername { get { return _LoggedInUserUsername; } set { _LoggedInUserUsername = value; DoPropertyChanged("LoggedInUserUsername"); } }
        public bool ToolbarVisible { get { return _ToolbarVisible; } set { _ToolbarVisible = value; DoPropertyChanged("ToolbarVisible"); } }
        public bool ToolbarNav_Visible { get { return _ToolbarNav_Visible; } set { _ToolbarNav_Visible = value; DoPropertyChanged("ToolbarNav_Visible"); } }
        public bool ToolbarArrangement_Visible { get { return _ToolbarArrangement_Visible; } set { _ToolbarArrangement_Visible = value; DoPropertyChanged("ToolbarArrangement_Visible"); } }
        public bool ToolbarPlace_Visible { get { return _ToolbarPlace_Visible; } set { _ToolbarPlace_Visible = value; DoPropertyChanged("ToolbarPlace_Visible"); } }
        public double StartLocation_Lat { get { return _StartLocation_Lat; } set { _StartLocation_Lat = value; DoPropertyChanged("StartLocation_Lat"); } }
        public double StartLocation_Long { get { return _StartLocation_Long; } set { _StartLocation_Long = value; DoPropertyChanged("StartLocation_Long"); } }

        private static readonly UserConfig _instance = new();
        private UserConfig()
        {
            LoadConfig();
        }
        public static UserConfig Instance { get { return _instance; } }

        private void LoadConfig()
        {
            List<string> lines;
            try
            {
                lines = File.ReadAllLines("./Data/UserConfig.preferences").ToList();
            }
            catch (FileNotFoundException)
            {
                Save();
                return;
			}

            foreach (var line in lines)
            {
                var tokens = line.Split("=", StringSplitOptions.TrimEntries);
                var key = tokens[0];
                var value = tokens[1];

                if (key == "LoggedInUserUsername")
                {
                    LoggedInUserUsername = value;
                }
                if (key == "ToolbarVisible")
                {
                    ToolbarVisible = bool.Parse(value.ToString());
                }
                if (key == "ToolbarNav_Visible")
                {
                    ToolbarNav_Visible = bool.Parse(value.ToString());
                }
                if (key == "ToolbarArrangement_Visible")
                {
                    ToolbarArrangement_Visible = bool.Parse(value.ToString());
                }
                if (key == "ToolbarPlace_Visible")
                {
                    ToolbarPlace_Visible = bool.Parse(value.ToString());
                }
                if (key == "StartLocation_Lat")
                {
                    StartLocation_Lat = double.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                }
                if (key == "StartLocation_Long")
                {
                    StartLocation_Long = double.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }

        public void Save()
        {
            string s = "";

            s += $"LoggedInUserUsername={LoggedInUserUsername}\n";
            s += $"ToolbarVisible={ToolbarVisible}\n";
            s += $"ToolbarNav_Visible={ToolbarNav_Visible}\n";
            s += $"ToolbarArrangement_Visible={ToolbarArrangement_Visible}\n";
            s += $"ToolbarPlace_Visible={ToolbarPlace_Visible}\n";

            s += $"StartLocation_Lat={StartLocation_Lat.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}\n";
            s += $"StartLocation_Long={StartLocation_Long.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}\n";

            File.WriteAllText("./Data/UserConfig.preferences", s);
        }
    }
}
