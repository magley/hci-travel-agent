using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace YouTravel.Util
{
    // Singleton.
    public class UserConfig
    {
		public ObservableCollection<ToolbarButton> PossibleToolbarButtons { get; set; } = new();
		public ObservableCollection<Button> ToolbarButtons { get; set; } = new();
		public bool ToolbarVisible { get; set; } = true;

		private static UserConfig _instance = new();
        private UserConfig() { }
        public static UserConfig Instance { get { return _instance; } }

        public void LoadToolbarConfig()
        {
            var lines = File.ReadAllLines("./Data/UserConfig.txt").ToList();

            foreach (var line in lines)
            {
                var tokens = line.Split("=", StringSplitOptions.TrimEntries);
                var key = tokens[0];
                var value = tokens[1];


                if (key == "ToolbarButtons")
                {
                    // Split by ',' and remove quotes between each entry.
                    var buttonNames = value
                        .Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Substring(1, x.Length - 2))
                        .ToList();

                    foreach (var possibleBtn in PossibleToolbarButtons)
                    {
                        if (buttonNames.Contains(possibleBtn.Name))
                        {
                            ToolbarButtons.Add(possibleBtn.Button);
                        }
                    }
                }
                if (key == "ToolbarVisible")
                {
                    ToolbarVisible = bool.Parse(value.ToString());
                }
            }
        }

        public void Save()
        {
            string s = "";

            s += $"ToolbarVisible={ToolbarVisible.ToString()}\n";

            s += "ToolbarButtons=";
            var buttonNamesToAdd = new List<string>();
            foreach (var btn in ToolbarButtons)
            {
                foreach(var pbtn in PossibleToolbarButtons)
                {
                    if (btn == pbtn.Button)
                    {
						buttonNamesToAdd.Add(pbtn.Name);
					}
                }
            }
            s += string.Join(",", buttonNamesToAdd.Select(b => $"\"{b}\""));
            s += "\n";

            File.WriteAllText("./Data/UserConfig.txt", s);
        }
    }
}
