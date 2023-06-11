using System.Windows;
using YouTravel.Shared;

namespace YouTravel.Util
{
    public class HelpProvider
    {
        public static string GetHelpKey(DependencyObject obj)
        {
            if (YouTravelContext.User?.Type == Model.UserType.AGENT)
            {
                return (string)obj.GetValue(HelpKeyPropertyAgent);
            } else
            {
                return (string)obj.GetValue(HelpKeyPropertyClient);
            }
        }

        public static void SetHelpKey(DependencyObject obj, string value)
        {
            obj.SetValue(HelpKeyPropertyAgent, value);
        }

        public static readonly DependencyProperty HelpKeyPropertyAgent = DependencyProperty.RegisterAttached(
            "HelpKeyAgent",
            typeof(string),
            typeof(HelpProvider),
            new PropertyMetadata("index_agent", HelpKey)
        );

        public static readonly DependencyProperty HelpKeyPropertyClient = DependencyProperty.RegisterAttached(
            "HelpKeyClient",
            typeof(string),
            typeof(HelpProvider),
            new PropertyMetadata("index_client", HelpKey)
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
