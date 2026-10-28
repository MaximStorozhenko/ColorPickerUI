using ColorPicker.Wpf.Helpers;
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
        #endregion

        private double hue = 210;
        private double saturation = 0.74;
        private double value = 0.96;

        private bool _isDragging;
        private bool _isUpdating;

        private bool _isUpdatingFromHsv;

        public ColorPicker()
        {
            InitializeComponent();
            UpdateVisuals();
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker picker)
                picker.UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (PreviewBorder is null || HexTextBox is null)
                return;

            PreviewBorder.Background = new SolidColorBrush(SelectedColor);
            HexTextBox.Text = ColorHelper.ToHex(SelectedColor);
            ErrorTextBlock.Visibility = Visibility.Collapsed;

            if (_isUpdatingFromHsv) return;

            (double Hue, double Saturation, double Value) hsv = ColorHelper.ToHsv(SelectedColor);

            _isUpdating = true;

            hue = hsv.Hue;
            saturation = hsv.Saturation;
            value = hsv.Value;

            HueSlider.Value = hue;

            _isUpdating = false;

            UpdateSvBackground();
            UpdateMarker();
        }

        private void SetColor(Color color)
        {
            SelectedColor = color;
        }

        private void PresetColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not string hex)
                return;

            if (ColorHelper.TryParseHex(hex, out Color color))
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
            Color rgb = ColorHelper.FromHsv(
                hue, saturation, value);

            _isUpdatingFromHsv = true;

            SetColor(Color.FromArgb(
                SelectedColor.A, rgb.R, rgb.G, rgb.B));

            _isUpdatingFromHsv = false;
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

        private void HueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUpdating || SvCanvas is null) return;

            hue = HueSlider.Value;

            UpdateSvBackground();
            UpdateSelectedColorFromHsv();
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
    }
}
