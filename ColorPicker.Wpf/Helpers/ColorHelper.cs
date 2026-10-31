using System.Globalization;
using System.Windows.Media;

namespace ColorPickerUI.Helpers
{
    internal static class ColorHelper
    {
        internal static string ToHex(Color color)
        {
            return color.A == 255
                ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        internal static bool TryParseHex(string? hex, out Color color)
        {
            color = Colors.Transparent;

            if(string.IsNullOrWhiteSpace(hex))
                return false;

            hex = hex.Trim();

            if (hex.StartsWith("#"))
                hex = hex[1..];

            if (!uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint value))
                return false;

            switch (hex.Length)
            {
                case 6:
                    color = Color.FromRgb(
                        (byte)(value >> 16),
                        (byte)(value >> 8),
                        (byte)value);
                    return true;
                case 8:
                    color = Color.FromArgb(
                        (byte)(value >> 24),
                        (byte)(value >> 16),
                        (byte)(value >> 8),
                        (byte)value);
                    return true;
                default:
                    return false;
            }
        }

        internal static Color FromHsv(double hue, double saturation, double value)
        {
            if (!double.IsFinite(hue))
                throw new ArgumentOutOfRangeException(nameof(hue));

            if (!double.IsFinite(saturation))
                throw new ArgumentOutOfRangeException(nameof(saturation));

            if (!double.IsFinite(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            hue = ((hue % 360) + 360) % 360;
            saturation = Math.Clamp(saturation, 0, 1);
            value = Math.Clamp(value, 0, 1);

            double chroma = value * saturation;
            double x = chroma * (1 - Math.Abs((hue / 60) % 2 - 1));
            double m = value - chroma;

            double r, g, b;

            if (hue < 60)
                (r, g, b) = (chroma, x, 0);
            else if (hue < 120)
                (r, g, b) = (x, chroma, 0);
            else if (hue < 180)
                (r, g, b) = (0, chroma, x);
            else if (hue < 240)
                (r, g, b) = (0, x, chroma);
            else if (hue < 300)
                (r, g, b) = (x, 0, chroma);
            else
                (r, g, b) = (chroma, 0, x);

            return Color.FromRgb(
                ToByte((r + m) * 255),
                ToByte((g + m) * 255),
                ToByte((b + m) * 255));
        }

        internal static (double Hue, double Saturation, double Value) ToHsv(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double hue = 0;

            if (delta > 0)
            {
                if (max == r)
                    hue = 60 * (((g - b) / delta) % 6);
                else if (max == g)
                    hue = 60 * ((b - r) / delta + 2);
                else
                    hue = 60 * ((r - g) / delta + 4);
            }

            if (hue < 0)
                hue += 360;

            double saturation = max == 0 ? 0 : delta / max;

            return (hue, saturation, max);
        }

        internal static byte ToByte(double value)
        {
            return (byte)Math.Clamp((int)Math.Round(value), 0, 255);
        }
    }
}
