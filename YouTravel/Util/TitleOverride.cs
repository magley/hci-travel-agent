using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace YouTravel.Util
{
    public class TitleOverride
    {
        public static void PageNameAsWords(Page page)
        {
            // HACK: This is kinda ugly but the alternative is create an abstract class for this one single thing which is worse.
            string pageNameAsWords = Regex.Replace(page.GetType().Name, "(\\B[A-Z])", " $1");
            if (pageNameAsWords != "")
            {
                pageNameAsWords = pageNameAsWords[0].ToString().ToUpper() + pageNameAsWords[1..].ToLower();
            }
            Window.GetWindow(page).Title = $"YouTravel - {pageNameAsWords}";
        }
    }
}
