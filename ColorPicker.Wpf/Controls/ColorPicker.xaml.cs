using ColorPicker.Wpf.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPicker.Wpf.Controls
{
    public partial class ColorPicker : UserControl
    {
        #region Settings
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
    }
}
