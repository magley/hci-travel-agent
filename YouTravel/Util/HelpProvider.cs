using System.Windows;
using YouTravel.Shared;

namespace YouTravel.Util
{
    public class HelpProvider
    {
        public static string GetHelpKey(DependencyObject obj)
        {
            return (string)obj.GetValue(HelpKeyProperty);
        }

        public static void SetHelpKey(DependencyObject obj, string value)
        {
            obj.SetValue(HelpKeyProperty, value);
        }

        public static readonly DependencyProperty HelpKeyProperty = DependencyProperty.RegisterAttached(
            "HelpKey",
            typeof(string),
            typeof(HelpProvider),
            new PropertyMetadata("index_agent", HelpKey)
        );

        private static void HelpKey(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static void ShowHelp(string key, Window originator)
        {
            HelpViewer hh = new(key)
            {
                Owner = originator
            };
            hh.Show();
        }
    }
}
