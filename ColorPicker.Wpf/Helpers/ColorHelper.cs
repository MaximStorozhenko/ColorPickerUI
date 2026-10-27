using System.Globalization;
using System.Windows.Media;

namespace ColorPicker.Wpf.Helpers
{
    public static class ColorHelper
    {
        public static string ToHex(Color color)
        {
            return color.A == 255
                ? $"#{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static bool TryParseHex(string? hex, out Color color)
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
    }
}
