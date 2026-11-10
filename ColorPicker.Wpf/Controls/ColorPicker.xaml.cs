using ColorPickerUI.Enums;
using ColorPickerUI.Helpers;
using ColorPickerUI.Models;
using ColorPickerUI.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ColorPickerUI.Controls
{
    /// <summary>
    /// A customizable WPF color picker control that allows users
    /// to select colors using HSV, RGB, HEX, and preset color palettes.
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        #region DependencyProperty
        /// <summary>
        /// Identifies the <see cref="SelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                nameof(SelectedColor),
                typeof(Color),
                typeof(ColorPicker),
                new FrameworkPropertyMetadata(
                    Colors.DodgerBlue,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedColorChanged));

        /// <summary>
        /// Gets or sets the currently selected color.
        /// </summary>
        /// <remarks>
        /// This property supports two-way data binding by default.
        /// </remarks>
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ShowPresetColors"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowPresetColorsProperty =
            DependencyProperty.Register(
                nameof(ShowPresetColors),
                typeof(Visibility),
                typeof(ColorPicker),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the visibility of the preset colors section.
        /// </summary>
        public Visibility ShowPresetColors
        {
            get => (Visibility)GetValue(ShowPresetColorsProperty);
            set => SetValue(ShowPresetColorsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Palette"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register(
                nameof(Palette),
                typeof(ColorPaletteType),
                typeof(ColorPicker),
                new PropertyMetadata(ColorPaletteType.Classic, OnPaletteChanged));

        /// <summary>
        /// Gets or sets the preset color palette displayed by the control.
        /// </summary>
        /// <remarks>
        /// Built-in palettes include Classic, Material, Pastel, Grayscale,
        /// and Extended. The Custom value allows using a user-defined palette.
        /// </remarks>
        public ColorPaletteType Palette
        {
            get => (ColorPaletteType)GetValue(PaletteProperty);
            set => SetValue(PaletteProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PresetColorShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PresetColorShapeProperty =
            DependencyProperty.Register(
                nameof(PresetColorShape),
                typeof(PresetColorShape),
                typeof(ColorPicker),
                new PropertyMetadata(PresetColorShape.Circle));

        /// <summary>
        /// Gets or sets the shape of the preset color items.
        /// </summary>
        public PresetColorShape PresetColorShape
        {
            get => (PresetColorShape)GetValue(PresetColorShapeProperty);
            set => SetValue(PresetColorShapeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PresetCustomColors"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PresetCustomColorsProperty =
            DependencyProperty.Register(
                nameof(PresetCustomColors),
                typeof(ObservableCollection<Color>),
                typeof(ColorPicker),
                new PropertyMetadata(null, OnPresetCustomColorsChanged));

        /// <summary>
        /// Gets or sets the collection of custom preset colors.
        /// </summary>
        /// <remarks>
        /// This collection is used when <see cref="Palette"/> is set to
        /// <see cref="ColorPaletteType.Custom"/>.
        /// Changes to the collection are automatically reflected in the control.
        /// The same collection can be shared between multiple ColorPicker instances.
        /// </remarks>
        public ObservableCollection<Color>? PresetCustomColors
        {
            get => (ObservableCollection<Color>?)GetValue(PresetCustomColorsProperty);
            set => SetValue(PresetCustomColorsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PresetColorSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PresetColorSizeProperty =
            DependencyProperty.Register(
                nameof(PresetColorSize),
                typeof(PresetColorSize),
                typeof(ColorPicker),
                new PropertyMetadata(PresetColorSize.Normal));

        /// <summary>
        /// Gets or sets the size of the preset color items.
        /// </summary>
        public PresetColorSize PresetColorSize
        {
            get => (PresetColorSize)GetValue(PresetColorSizeProperty);
            set => SetValue(PresetColorSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ShowPreviewSelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowPreviewSelectedColorProperty =
            DependencyProperty.Register(
                nameof(ShowPreviewSelectedColor),
                typeof(Visibility),
                typeof(ColorPicker),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets a value indicating whether the selected color preview is visible.
        /// </summary>
        public Visibility ShowPreviewSelectedColor
        {
            get => (Visibility)GetValue(ShowPreviewSelectedColorProperty);
            set => SetValue(ShowPreviewSelectedColorProperty, value);
        }
        #endregion

        private double hue = 210;
        private double saturation = 0.74;
        private double value = 0.96;

        private bool _isDragging;
        private bool _isUpdating;

        private bool _isUpdatingFromHsv;

        private EyedropperWindow? eyedropperWindow;

        /// <summary>
        /// Gets the collection of preset colors currently displayed by the control.
        /// </summary>
        /// <remarks>
        /// This collection is updated automatically when the selected palette
        /// or custom preset colors change.
        /// </remarks>
        public ObservableCollection<Color> DisplayedPresetColors { get; } = new();

        /// <summary>
        /// The current color format used for clipboard copying.
        /// </summary>
        private CopyColorFormat copyFormat = CopyColorFormat.HEX;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPicker"/> control.
        /// </summary>
        public ColorPicker()
        {
            InitializeComponent();

            SetCurrentValue(PresetCustomColorsProperty, new ObservableCollection<Color>());

            UpdateVisuals();
            UpdatePresetColors();

            CopyIcon.Visibility = Visibility.Visible;
            CopySuccessIcon.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Updates the collection of displayed preset colors
        /// according to the currently selected palette.
        /// </summary>
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

        /// <summary>
        /// Handles changes to the selected color palette
        /// and refreshes the displayed preset colors.
        /// </summary>
        private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker picker)
                picker.UpdatePresetColors();
        }

        /// <summary>
        /// Handles changes to the selected color
        /// and updates the visual elements of the control.
        /// </summary>
        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker picker)
                picker.UpdateVisuals();
        }

        /// <summary>
        /// Handles changes to the custom preset colors collection,
        /// updates collection change subscriptions, and refreshes displayed colors.
        /// </summary>
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

        /// <summary>
        /// Handles changes within the custom preset colors collection
        /// and refreshes the displayed colors.
        /// </summary>
        private void PresetCustomColors_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePresetColors();
        }

        /// <summary>
        /// Synchronizes the control's visual elements with the currently selected color,
        /// including the preview, HEX input, sliders, gradients, and selection marker.
        /// </summary>
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

        /// <summary>
        /// Sets the selected color and triggers the corresponding visual updates.
        /// </summary>
        /// <param name="color">The color to apply.</param>
        private void SetColor(Color color)
        {
            SelectedColor = color;
        }

        /// <summary>
        /// Applies the color associated with the clicked preset color button.
        /// </summary>
        private void PresetColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Color color)
                SetColor(color);
        }

        /// <summary>
        /// Applies the color entered in the HEX input field
        /// when the Apply button is clicked.
        /// </summary>
        private void ApplyHexButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyHex();
        }

        /// <summary>
        /// Applies the entered HEX color when the Enter key is pressed.
        /// </summary>
        private void HexTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyHex();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Parses and applies the HEX color entered by the user.
        /// Displays an error message if the input is invalid.
        /// </summary>
        private void ApplyHex()
        {
            if (ColorHelper.TryParseHex(HexTextBox.Text, out Color color))
                SetColor(color);
            else
                ErrorTextBlock.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Updates the saturation and value selection area background
        /// using the current hue.
        /// </summary>
        private void UpdateSvBackground()
        {
            if (SvCanvas is null)
                return;

            SvCanvas.Background = new SolidColorBrush(
                ColorHelper.FromHsv(hue, 1, 1));
        }

        /// <summary>
        /// Updates the position of the selection marker
        /// according to the current saturation and value.
        /// </summary>
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

        /// <summary>
        /// Converts the current HSV values to RGB
        /// and updates the selected color while preserving its alpha channel.
        /// </summary>
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

        /// <summary>
        /// Calculates saturation and value from the specified position
        /// within the saturation and value selection area.
        /// Updates the selection marker and selected color.
        /// </summary>
        /// <param name="position">The pointer position relative to the selection area.</param>
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

        /// <summary>
        /// Handles HSV slider value changes and updates the selected color
        /// and related visual elements.
        /// </summary>
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

        /// <summary>
        /// Handles RGB slider value changes and updates the selected color.
        /// </summary>
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

        /// <summary>
        /// Handles alpha slider value changes and updates the transparency
        /// of the selected color.
        /// </summary>
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

        /// <summary>
        /// Begins color selection by capturing the mouse
        /// and processing the initial pointer position.
        /// </summary>
        private void SvCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            SvCanvas.CaptureMouse();

            SelectColorAt(e.GetPosition(SvCanvas));
            e.Handled = true;
        }

        /// <summary>
        /// Updates the selected color while the user drags
        /// the mouse within the saturation and value selection area.
        /// </summary>
        private void SvCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
                SelectColorAt(e.GetPosition(SvCanvas));
        }

        /// <summary>
        /// Repositions the selection marker when the selection area is resized.
        /// </summary>
        private void SvCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMarker();
        }

        /// <summary>
        /// Ends the color selection operation and releases mouse capture.
        /// </summary>
        private void SvCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            SvCanvas.ReleaseMouseCapture();
        }

        /// <summary>
        /// Resets the dragging state when the selection area loses mouse capture.
        /// </summary>
        private void SvCanvas_LostMouseCapture(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        /// <summary>
        /// Updates the background gradients of the saturation, value,
        /// and alpha sliders according to the current color.
        /// </summary>
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

        /// <summary>
        /// Copies the currently selected color to the clipboard
        /// using the selected copy format.
        /// </summary>
        private async void CopyColorButton_Click(object sender, RoutedEventArgs e)
        {
            Color color = SelectedColor;

            string text = copyFormat switch
            {
                CopyColorFormat.HEX =>
                    $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}",

                CopyColorFormat.RGB =>
                    $"{color.R}, {color.G}, {color.B}",

                CopyColorFormat.ARGB =>
                    $"{color.A}, {color.R}, {color.G}, {color.B}",

                _ => string.Empty
            };

            IntPtr hwnd = new WindowInteropHelper(Window.GetWindow(this)).Handle;

            bool copied = ClipboardService.TryCopyText(text, hwnd);

            if (!copied)
            {
                Debug.WriteLine("Clipboard занят. Копирование пропущено.");
                return;
            }

            CopyIcon.Visibility = Visibility.Collapsed;
            CopySuccessIcon.Visibility = Visibility.Visible;

            await Task.Delay(700);

            CopyIcon.Visibility = Visibility.Visible;
            CopySuccessIcon.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Opens the context menu with available color copy formats.
        /// </summary>
        private void CopyFormatButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
                button.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// Updates the selected color copy format based on the chosen menu item.
        /// </summary>
        private void CopyFormatMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                Enum.TryParse(menuItem.Tag?.ToString(), out CopyColorFormat format))
                copyFormat = format;
        }

        /// <summary>
        /// Handles the eyedropper button click and starts the color picking process.
        /// </summary>
        private void EyedropperButton_Click(object sender, RoutedEventArgs e)
        {
            if (eyedropperWindow != null)
            {
                if (eyedropperWindow.IsVisible)
                    return;

                eyedropperWindow = null;
            }

            (int X, int Y) point = EyedropperService.GetCursorPosition();

            BitmapSource screenshot = EyedropperService.CaptureScreenArea(point.X - 7, point.Y - 7, 15, 15);

            eyedropperWindow = new();
            eyedropperWindow.Owner = Window.GetWindow(this);
            eyedropperWindow.ColorPicked += color =>
            {
                SelectedColor = color;
            };
            eyedropperWindow.Closed += (_, _) =>
            {
                eyedropperWindow = null;
            };
            eyedropperWindow.SetInitialPosition(point.X, point.Y);
            eyedropperWindow.Show();
            eyedropperWindow.UpdateMagnifier(screenshot);
        }
    }
}
