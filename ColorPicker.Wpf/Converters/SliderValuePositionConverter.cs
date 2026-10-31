using System.Globalization;
using System.Windows.Data;

namespace ColorPicker.Wpf.Converters
{
    internal class SliderValuePositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 4 ||
                values[0] is not double value ||
                values[1] is not double minimum ||
                values[2] is not double maximum ||
                values[3] is not double width ||
                maximum <= minimum ||
                width <= 0) return 0.0;

            const double ThumbWidth = 50;
            const double IndicatorWidth = 35;

            double ratio = Math.Clamp((value - minimum) / (maximum - minimum), 0, 1);

            double thumbPosition = ThumbWidth / 2 + ratio * (width - ThumbWidth);

            double left = thumbPosition - IndicatorWidth / 2;

            return Math.Clamp(left, 0, Math.Max(0, width - IndicatorWidth));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes
                .Select(_ => Binding.DoNothing)
                .ToArray();
        }
    }
}
