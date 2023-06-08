using System;
using System.Media;
using System.Windows;

namespace YouTravel.Util
{
    public static class SoundUtil
    {
        public static void PlaySound(string fname)
        {
            var uri = Application.GetResourceStream(new Uri($"pack://application:,,,/Res/{fname}"));
            SoundPlayer player = new SoundPlayer(uri.Stream);
            player.Load();
            player.Play();
        }
    }
}
