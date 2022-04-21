using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MarillOMeter.Utils
{
    internal static class ColorUtils
    {
        private static Random rand = new Random();

        private static Color[] colors = new[] {
            Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 255, 120, 0),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 150, 255, 0),
            Color.FromArgb(255, 0, 255, 0),
            Color.FromArgb(255, 0, 255, 255),
            Color.FromArgb(255, 0, 0, 255),
            Color.FromArgb(255, 255, 0, 255),
            Color.FromArgb(255, 255, 0, 150)
        };

        public static Color RandomBrightColor() =>
            colors[rand.Next() % colors.Length];

        public static Brush RandomBrightBrush() =>
            new SolidColorBrush(ColorUtils.RandomBrightColor());
    }
}
