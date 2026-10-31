using System.Windows.Media;

namespace ColorPicker.Wpf.Models
{
    public sealed class ColorPalette
    {
        public string Name { get; set; } = string.Empty;
        public List<Color> Colors { get; set; } = new();
    }
}