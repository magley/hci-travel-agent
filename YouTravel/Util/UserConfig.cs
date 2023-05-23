using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace YouTravel.Util
{
    // Singleton.
    public class UserConfig
    {
		public ObservableCollection<ToolbarButton> PossibleToolbarButtons { get; set; } = new();
		public ObservableCollection<Button> ToolbarButtons { get; set; } = new();

		private static UserConfig _instance = new();
        private UserConfig() { }
        public static UserConfig Instance { get { return _instance; } }
    }
}
