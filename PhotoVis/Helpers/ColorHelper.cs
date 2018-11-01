using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PhotoVis.Helpers
{
    class ColorHelper
    {

        public static Color GetBlendedDarkerColor(double percentage)
        {
            if (percentage < 0.5)
                return Interpolate(Color.DarkRed, Color.Gold, percentage);
            return Interpolate(Color.Gold, Color.Green, (percentage - 0.5));
        }

        public static Color GetBlendedColor(double percentage)
        {
            if (percentage < 0.5)
                return Interpolate(Color.Red, Color.Yellow, percentage);
            return Interpolate(Color.Yellow, Color.LimeGreen, (percentage - 0.5));
        }

        private static Color Interpolate(Color color1, Color color2, double fraction)
        {
            double r = Interpolate(color1.R, color2.R, fraction);
            double g = Interpolate(color1.G, color2.G, fraction);
            double b = Interpolate(color1.B, color2.B, fraction);
            return Color.FromArgb((int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b));
        }

        private static double Interpolate(double d1, double d2, double fraction)
        {
            return d1 + (d2 - d1) * fraction;
        }

        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static Color HSL2RGB(double alpha, double h, double sl, double l)
        {
            double v;
            double r, g, b;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            Color rgb = Color.FromArgb(
                 (int)(alpha * 255.0f),
                 (int)(r * 255.0f),
                 (int)(g * 255.0f),
                 (int)(b * 255.0f)
                );

            //Color rgb = new Color();
            //rgb.A = Convert.ToByte(alpha * 255.0f);
            //rgb.R = Convert.ToByte(r * 255.0f);
            //rgb.G = Convert.ToByte(g * 255.0f);
            //rgb.B = Convert.ToByte(b * 255.0f);
            return rgb;
        }

        public static Color Rainbow(float progress)
        {
            float div = (Math.Abs(progress % 1) * 6);
            int ascending = (int)((div % 1) * 255);
            int descending = 255 - ascending;

            switch ((int)div)
            {
                case 0:
                    return Color.FromArgb(255, 255, ascending, 0);
                case 1:
                    return Color.FromArgb(255, descending, 255, 0);
                case 2:
                    return Color.FromArgb(255, 0, 255, ascending);
                case 3:
                    return Color.FromArgb(255, 0, descending, 255);
                case 4:
                    return Color.FromArgb(255, ascending, 0, 255);
                default: // case 5:
                    return Color.FromArgb(255, 255, 0, descending);
            }
        }

        public static Color RainBowColor(double value, double maxValue, int lightness = 128)
        {
            var i = (int)(value * 255 / maxValue);
            var r = (int)Math.Round(Math.Sin(0.024 * i + 0) * 127 + lightness);
            var g = (int)Math.Round(Math.Sin(0.024 * i + 2) * 127 + lightness);
            var b = (int)Math.Round(Math.Sin(0.024 * i + 4) * 127 + lightness);
            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }

        //private static string HexConverter(Color c)
        //{
        //    return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        //}
    }
}
