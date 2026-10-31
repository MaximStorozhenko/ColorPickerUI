using ColorPicker.Wpf.Enums;
using ColorPicker.Wpf.Helpers;
using ColorPicker.Wpf.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker.Wpf.Controls
{
    public partial class ColorPicker : UserControl
    {
        #region DependencyProperty
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(Color),
                typeof(ColorPicker),
                new FrameworkPropertyMetadata(
                    Colors.DodgerBlue,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedColorChanged));

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public static readonly DependencyProperty ShowPresetColorsProperty =
            DependencyProperty.Register(
                nameof(ShowPresetColors),
                typeof(Visibility),
                typeof(ColorPicker),
                new PropertyMetadata(Visibility.Visible));

        public Visibility ShowPresetColors
        {
            get => (Visibility)GetValue(ShowPresetColorsProperty);
            set => SetValue(ShowPresetColorsProperty, value);
        }

        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register(
                nameof(Palette),
                typeof(ColorPaletteType),
                typeof(ColorPicker),
                new PropertyMetadata(ColorPaletteType.Classic, OnPaletteChanged));

        public ColorPaletteType Palette
        {
            get => (ColorPaletteType)GetValue(PaletteProperty);
            set => SetValue(PaletteProperty, value);
        }

        public static readonly DependencyProperty PresetColorShapeProperty =
            DependencyProperty.Register(
                nameof(PresetColorShape),
                typeof(PresetColorShape),
                typeof(ColorPicker),
                new PropertyMetadata(PresetColorShape.Circle));

        public PresetColorShape PresetColorShape
        {
            get => (PresetColorShape)GetValue(PresetColorShapeProperty);
            set => SetValue(PresetColorShapeProperty, value);
        }

        public static readonly DependencyProperty PresetCustomColorsProperty =
            DependencyProperty.Register(
                nameof(PresetCustomColors),
                typeof(ObservableCollection<Color>),
                typeof(ColorPicker),
                new PropertyMetadata(null, OnPresetCustomColorsChanged));

        public ObservableCollection<Color>? PresetCustomColors
        {
            get => (ObservableCollection<Color>?)GetValue(PresetCustomColorsProperty);
            set => SetValue(PresetCustomColorsProperty, value);
        }
        #endregion

        private double hue = 210;
        private double saturation = 0.74;
        private double value = 0.96;

        private bool _isDragging;
        private bool _isUpdating;

        private bool _isUpdatingFromHsv;
                
        public ObservableCollection<Color> DisplayedPresetColors { get; } = new();

        public ColorPicker()
        {
            InitializeComponent();

            SetCurrentValue(PresetCustomColorsProperty, new ObservableCollection<Color>());

            UpdateVisuals();
            UpdatePresetColors();
        }

        private void UpdatePresetColors()
        {
            DisplayedPresetColors.Clear();

            if(Palette == ColorPaletteType.Custom)
            {
                if(PresetCustomColors is not null)
                    foreach(Color color in PresetCustomColors)
                        DisplayedPresetColors.Add(color);

                return;
            }

            if(ColorPalettes.All.TryGetValue(Palette, out ColorPalette palette))
                foreach(Color color in palette.Colors)
                    DisplayedPresetColors.Add(color);
        }

        private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker picker)
                picker.UpdatePresetColors();
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker picker)
                picker.UpdateVisuals();
        }

        private static void OnPresetCustomColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPicker picker)
                return;

            if (e.OldValue is ObservableCollection<Color> oldColors)
                oldColors.CollectionChanged -= picker.PresetCustomColors_CollectionChanged;

            if (e.NewValue is ObservableCollection<Color> newColors)
                newColors.CollectionChanged += picker.PresetCustomColors_CollectionChanged;

            picker.UpdatePresetColors();
        }

        private void PresetCustomColors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePresetColors();
        }

        private void UpdateVisuals()
        {
            if (PreviewBorder is null || HexTextBox is null || ErrorTextBlock is null ||
                HueSlider is null || SaturationSlider is null || ValueSlider is null ||
                RedSlider is null || GreenSlider is null || BlueSlider is null ||
                AlphaSlider is null || SvCanvas is null || SvMarker is null) return;

            PreviewBorder.Background = new SolidColorBrush(SelectedColor);
            HexTextBox.Text = ColorHelper.ToHex(SelectedColor);
            ErrorTextBlock.Visibility = Visibility.Collapsed;

            _isUpdating = true;

            try
            {
                if (!_isUpdatingFromHsv)
                {
                    (double Hue, double Saturation, double Value) hsv = ColorHelper.ToHsv(SelectedColor);

                    hue = hsv.Hue;
                    saturation = hsv.Saturation;
                    value = hsv.Value;
                }

                HueSlider.Value = hue;
                SaturationSlider.Value = saturation * 100;
                ValueSlider.Value = value * 100;

                RedSlider.Value = SelectedColor.R;
                GreenSlider.Value = SelectedColor.G;
                BlueSlider.Value = SelectedColor.B;
                AlphaSlider.Value = SelectedColor.A;

                UpdateSliderGradients();
                UpdateSvBackground();
                UpdateMarker();
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void SetColor(Color color)
        {
            SelectedColor = color;
        }

        private void PresetColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Color color)
                SetColor(color);
        }

        private void ApplyHexButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyHex();
        }

        private void HexTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyHex();
                e.Handled = true;
            }
        }

        private void ApplyHex()
        {
            if (ColorHelper.TryParseHex(HexTextBox.Text, out Color color))
                SetColor(color);
            else
                ErrorTextBlock.Visibility = Visibility.Visible;
        }

        private void UpdateSvBackground()
        {
            if (SvCanvas is null)
                return;

            SvCanvas.Background = new SolidColorBrush(
                ColorHelper.FromHsv(hue, 1, 1));
        }

        private void UpdateMarker()
        {
            if (SvCanvas is null || SvMarker is null)
                return;

            double x = saturation * SvCanvas.ActualWidth;
            double y = (1 - value) * SvCanvas.ActualHeight;

            Canvas.SetLeft(SvMarker,
                Math.Clamp(x - SvMarker.Width / 2, 0,
                Math.Max(0, SvCanvas.ActualWidth - SvMarker.Width)));

            Canvas.SetTop(SvMarker,
                Math.Clamp(y - SvMarker.Height / 2, 0,
                Math.Max(0, SvCanvas.ActualHeight - SvMarker.Height)));
        }

        private void UpdateSelectedColorFromHsv()
        {
            Color rgb = ColorHelper.FromHsv(hue, saturation, value);

            _isUpdatingFromHsv = true;

            try
            {
                SetColor(Color.FromArgb((byte)AlphaSlider.Value, rgb.R, rgb.G, rgb.B));
            }
            finally
            {
                _isUpdatingFromHsv = false;
            }
        }

        private void SelectColorAt(Point position)
        {
            double width = SvCanvas.ActualWidth;
            double height = SvCanvas.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double x = Math.Clamp(position.X, 0, width);
            double y = Math.Clamp(position.Y, 0, height);

            saturation = x / width;
            value = 1.0 - y / height;

            UpdateMarker();
            UpdateSelectedColorFromHsv();
        }

        private void HsvSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdating || HueSlider is null || SaturationSlider is null ||
                ValueSlider is null || AlphaSlider is null || SvCanvas is null) return;

            hue = HueSlider.Value;
            saturation = SaturationSlider.Value / 100.0;
            value = ValueSlider.Value / 100.0;

            UpdateSvBackground();
            UpdateSelectedColorFromHsv();
            UpdateMarker();
        }

        private void RgbSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdating || RedSlider is null || GreenSlider is null ||
                BlueSlider is null || AlphaSlider is null) return;

                SetColor(Color.FromArgb(
                (byte)AlphaSlider.Value,
                (byte)RedSlider.Value,
                (byte)GreenSlider.Value,
                (byte)BlueSlider.Value));
        }

        private void AlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdating || AlphaSlider is null)
                return;

            SetColor(Color.FromArgb(
                (byte)AlphaSlider.Value,
                SelectedColor.R,
                SelectedColor.G,
                SelectedColor.B));
        }

        private void SvCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            SvCanvas.CaptureMouse();

            SelectColorAt(e.GetPosition(SvCanvas));
            e.Handled = true;
        }

        private void SvCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
                SelectColorAt(e.GetPosition(SvCanvas));
        }

        private void SvCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMarker();
        }

        private void SvCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            SvCanvas.ReleaseMouseCapture();
        }

        private void SvCanvas_LostMouseCapture(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void UpdateSliderGradients()
        {
            Color currentSaturation = ColorHelper.FromHsv(hue, 1, value);
            Color currentValue = ColorHelper.FromHsv(hue, saturation, 1);

            SaturationSlider.Background = new LinearGradientBrush(
                ColorHelper.FromHsv(hue, 0, value), currentSaturation,
                new Point(0, 0.5), new Point(1, 0.5));

            ValueSlider.Background = new LinearGradientBrush(
                Colors.Black, currentValue,
                new Point(0, 0.5), new Point(1, 0.5));

            Color alphaColor = SelectedColor;

            AlphaSlider.Background = new LinearGradientBrush(
                Color.FromArgb(0, alphaColor.R, alphaColor.G, alphaColor.B),
                Color.FromArgb(255, alphaColor.R, alphaColor.G, alphaColor.B),
                new Point(0, 0.5), new Point(1, 0.5));
        }
    }
}
