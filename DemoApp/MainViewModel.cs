using ColorPickerUI.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Input;

namespace DemoApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Color selectedColor = Colors.DodgerBlue;

        public Color SelectedColor
        {
            get => selectedColor;
            set
            {
                if (selectedColor == value) return;
                selectedColor = value;
                OnPropertyChanged();
            }
        }

        private Visibility showPreviewColor = Visibility.Visible;

        public Visibility ShowPreviewColor
        {
            get => showPreviewColor;
            set
            {
                if (showPreviewColor == value) return;
                showPreviewColor = value;
                OnPropertyChanged();
            }
        }

        private Visibility showPresetColor = Visibility.Visible;
        public Visibility ShowPresetColor
        {
            get => showPresetColor;
            set
            {
                if (showPresetColor == value) return;
                showPresetColor = value;
                OnPropertyChanged();
            }
        }

        private ColorPaletteType colorPalette = ColorPaletteType.Classic;
        public ColorPaletteType ColorPalette
        {
            get => colorPalette;
            set
            {
                if (colorPalette == value) return;
                colorPalette = value;
                OnPropertyChanged();
            }
        }

        private string hexInput = string.Empty;
        public string HexInput
        {
            get => hexInput;
            set
            {
                if (hexInput == value) return;
                hexInput = value;
                OnPropertyChanged();
            }
        }

        private PresetColorShape presetColorShape = PresetColorShape.Circle;

        public PresetColorShape PresetColorShape
        {
            get => presetColorShape;
            set
            {
                if (presetColorShape == value) return;
                presetColorShape = value;
                OnPropertyChanged();
            }
        }

        private PresetColorSize presetColorSize = PresetColorSize.Normal;
        public PresetColorSize PresetColorSize
        {
            get => presetColorSize;
            set
            {
                if (presetColorSize == value) return;
                presetColorSize = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Color> CustomColors { get; } = new()
        {
            Colors.Red,
            Colors.Green,
            Colors.Blue,
            Colors.Yellow,
            Colors.Orange,
            Colors.Purple
        };

        public RelayCommand<string> SetColorCommand { get; }
        public RelayCommand<string> SetVisibilityPreviewCommand { get; }
        public RelayCommand<string> SetVisibilityPresetColorsCommand { get; }
        public RelayCommand<string> SetPaletteCommand { get; }
        public RelayCommand<object> AddHexButtonCommand { get; }
        public RelayCommand<string> SetPresetColorShapeCommand { get; }
        public RelayCommand<string> SetPresetColorSizeCommand { get; }

        public MainViewModel()
        {
            SetColorCommand = new RelayCommand<string>(SetColor);
            SetVisibilityPreviewCommand = new RelayCommand<string>(SetVisibilityPreview);
            SetVisibilityPresetColorsCommand = new RelayCommand<string>(SetVisibilityPresetColors);
            SetPaletteCommand = new RelayCommand<string>(SetPalette);
            AddHexButtonCommand = new RelayCommand<object>(AddHexColor);
            SetPresetColorShapeCommand = new RelayCommand<string>(SetPresetColorShape);
            SetPresetColorSizeCommand = new RelayCommand<string>(SetPresetColorSize);
        }

        private void SetPresetColorSize(string? size)
        {
            if(string.IsNullOrEmpty(size)) return;

            if(Enum.TryParse(size, true, out PresetColorSize result))
                PresetColorSize = result;
        }

        private void SetPresetColorShape(string? shape)
        {
            if (string.IsNullOrEmpty(shape)) return;

            if (Enum.TryParse(shape, true, out PresetColorShape result))
                PresetColorShape = result;
        }

        private void AddHexColor(object? _)
        {
            if (string.IsNullOrWhiteSpace(HexInput)) return;

            try
            {
                if (ColorConverter.ConvertFromString(HexInput) is not Color color) return;

                if (CustomColors.Contains(color))
                {
                    MessageBox.Show($"Color {HexInput} already exists in the custom colors.", "Duplicate Color",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                CustomColors.Add(color);
                HexInput = string.Empty;
            }
            catch (FormatException)
            {
                MessageBox.Show($"Invalid HEX color: {HexInput}", "Invalid Color",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SetPalette(string? palette)
        {
            if(Enum.TryParse(palette, true, out ColorPaletteType result))
                ColorPalette = result;
        }

        private void SetVisibilityPresetColors(string? visibility)
        {
            if (Enum.TryParse(visibility, true, out Visibility result))
                ShowPresetColor = result;
        }

        private void SetVisibilityPreview(string? visibility)
        {
            if (Enum.TryParse(visibility, true, out Visibility result))
                ShowPreviewColor = result;
        }

        private void SetColor(string? strColor)
        {
            if (string.IsNullOrWhiteSpace(strColor)) return;

            try
            {
                if (ColorConverter.ConvertFromString(strColor) is not Color color) return;

                SelectedColor = color;
            }
            catch (FormatException)
            {
                MessageBox.Show($"Invalid color: {strColor}", "Invalid Color",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
