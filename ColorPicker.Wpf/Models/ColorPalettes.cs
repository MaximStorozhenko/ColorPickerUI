using ColorPicker.Wpf.Enums;
using System.Windows.Media;

namespace ColorPicker.Wpf.Models
{
    public class ColorPalettes
    {
        public static readonly Dictionary<ColorPaletteType, ColorPalette> All = new()
        {
            [ColorPaletteType.Classic] = new ColorPalette
            {
                Name = "Classic",
                Colors = new List<Color>
                {
                    Color.FromRgb(244, 67, 54),
                    Color.FromRgb(233, 30, 99),
                    Color.FromRgb(156, 39, 176),
                    Color.FromRgb(103, 58, 183),
                    Color.FromRgb(63, 81, 181),
                    Color.FromRgb(33, 150, 243),
                    Color.FromRgb(3, 169, 244),
                    Color.FromRgb(0, 188, 212),
                    Color.FromRgb(0, 150, 136),
                    Color.FromRgb(76, 175, 80),
                    Color.FromRgb(139, 195, 74),
                    Color.FromRgb(205, 220, 57),
                    Color.FromRgb(255, 235, 59),
                    Color.FromRgb(255, 193, 7),
                    Color.FromRgb(255, 152, 0),
                    Color.FromRgb(255, 87, 34)
                }
            },

            [ColorPaletteType.Material] = new ColorPalette
            {
                Name = "Material",
                Colors = new List<Color>
                {
                    Color.FromRgb(183, 28, 28),
                    Color.FromRgb(194, 24, 91),
                    Color.FromRgb(123, 31, 162),
                    Color.FromRgb(81, 45, 168),
                    Color.FromRgb(48, 63, 159),
                    Color.FromRgb(21, 101, 192),
                    Color.FromRgb(2, 119, 189),
                    Color.FromRgb(0, 131, 143),
                    Color.FromRgb(0, 105, 92),
                    Color.FromRgb(46, 125, 50),
                    Color.FromRgb(85, 139, 47),
                    Color.FromRgb(158, 157, 36),
                    Color.FromRgb(249, 168, 37),
                    Color.FromRgb(255, 143, 0),
                    Color.FromRgb(230, 81, 0),
                    Color.FromRgb(191, 54, 12)
                }
            },

            [ColorPaletteType.Pastel] = new ColorPalette
            {
                Name = "Pastel",
                Colors = new List<Color>
                {
                    Color.FromRgb(255, 179, 186),
                    Color.FromRgb(255, 223, 186),
                    Color.FromRgb(255, 255, 186),
                    Color.FromRgb(186, 255, 201),
                    Color.FromRgb(186, 225, 255),
                    Color.FromRgb(202, 196, 255),
                    Color.FromRgb(255, 198, 255),
                    Color.FromRgb(255, 218, 233),
                    Color.FromRgb(255, 204, 188),
                    Color.FromRgb(255, 229, 180),
                    Color.FromRgb(218, 255, 180),
                    Color.FromRgb(180, 255, 231),
                    Color.FromRgb(180, 229, 255),
                    Color.FromRgb(218, 180, 255),
                    Color.FromRgb(255, 180, 218),
                    Color.FromRgb(240, 240, 240)
                }
            },

            [ColorPaletteType.Grayscale] = new ColorPalette
            {
                Name = "Grayscale",
                Colors = Enumerable.Range(0, 16)
                .Select(i =>
                {
                    byte value = (byte)(i * 17);
                    return Color.FromRgb(value, value, value);
                })
                .ToList()
            },

            [ColorPaletteType.Extended] = new ColorPalette
            {
                Name = "Extended",
                Colors = new List<Color>
                {
                    Color.FromRgb(128, 0, 0),
                    Color.FromRgb(128, 64, 0),
                    Color.FromRgb(128, 128, 0),
                    Color.FromRgb(0, 128, 0),
                    Color.FromRgb(0, 128, 128),
                    Color.FromRgb(0, 64, 128),
                    Color.FromRgb(0, 0, 128),
                    Color.FromRgb(64, 0, 128),
                    Color.FromRgb(255, 105, 97),
                    Color.FromRgb(255, 179, 71),
                    Color.FromRgb(255, 250, 101),
                    Color.FromRgb(119, 221, 119),
                    Color.FromRgb(119, 158, 203),
                    Color.FromRgb(174, 198, 207),
                    Color.FromRgb(179, 158, 181),
                    Color.FromRgb(203, 153, 201)
                }
            }
        };
    }
}
