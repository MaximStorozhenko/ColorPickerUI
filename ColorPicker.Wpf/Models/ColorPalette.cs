using System.Windows.Media;

namespace ColorPickerUI.Models
{
    public sealed class ColorPalette
    {
        public string Name { get; set; } = string.Empty;
        public List<Color> Colors { get; set; } = new();
    }
}