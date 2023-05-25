using System;
using System.IO;
using System.Linq;

namespace YouTravel.Util
{
    // Singleton.
    public class UserConfig
    {
        public bool ToolbarVisible { get; set; } = true;
        public bool ToolbarNav_Visible { get; set; } = true;
        public bool ToolbarArrangement_Visible { get; set; } = true;
        public bool ToolbarPlace_Visible { get; set; } = true;

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
			}

            Console.WriteLine(ToolbarVisible);
		}

		public void Save()
		{
            string s = "";

            s += $"ToolbarVisible={ToolbarVisible}\n";
			s += $"ToolbarNav_Visible={ToolbarNav_Visible}\n";
			s += $"ToolbarArrangement_Visible={ToolbarArrangement_Visible}\n";
			s += $"ToolbarPlace_Visible={ToolbarPlace_Visible}\n";

            File.WriteAllText("./Data/UserConfig.txt", s);
        }
    }
}
