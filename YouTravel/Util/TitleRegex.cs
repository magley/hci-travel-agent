using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace YouTravel.Util
{
    public class TitleRegex
    {
        public static string PageNameAsWords(FrameworkElement element)
        {
            // HACK: This is kinda ugly but the alternative is create an abstract class for this one single thing which is worse.
            string pageNameAsWords = Regex.Replace(element.GetType().Name, "(\\B[A-Z])", " $1");
            if (pageNameAsWords != "")
            {
                pageNameAsWords = pageNameAsWords[0].ToString().ToUpper() + pageNameAsWords.Substring(1).ToLower();
            }
            return pageNameAsWords;
        }
    }
}
