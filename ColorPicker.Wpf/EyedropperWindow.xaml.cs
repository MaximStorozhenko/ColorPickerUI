using ColorPickerUI.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ColorPickerUI
{
    internal partial class EyedropperWindow : Window
    {
        private readonly DispatcherTimer timer;

        /// <summary>
        /// Action when a color is selected using the eyedropper.
        /// </summary>
        public event Action<Color>? ColorPicked;

        private bool wasEscapeDown;

        private readonly MouseHookService mouseHookService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EyedropperWindow"/> class.
        /// </summary>
        public EyedropperWindow()
        {
            InitializeComponent();

            mouseHookService = new MouseHookService();
            mouseHookService.LeftButtonDown += MouseHook_LeftButtonDown;
            mouseHookService.Start();

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            timer.Tick += Timer_Tick;
            timer.Start();
        }

        /// <summary>
        /// Sets the initial position of the eyedropper window relative to the cursor.
        /// </summary>
        /// <param name="cursorX">The horizontal screen coordinate of the cursor.</param>
        /// <param name="cursorY">The vertical screen coordinate of the cursor.</param>
        public void SetInitialPosition(int cursorX, int cursorY)
        {
            UpdateWindowPosition(cursorX, cursorY);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            bool escapeDown = EyedropperService.IsEscapePressed();

            if(escapeDown && !wasEscapeDown)
            {
                Close();
                return;
            }

            wasEscapeDown = escapeDown;

            (int x, int y) = EyedropperService.GetCursorPosition();

            MagnifierImage.Source = EyedropperService.CaptureAroundCursor();

            UpdateWindowPosition(x, y);
        }

        /// <summary>
        /// Updates the magnifier image with the specified screen capture.
        /// </summary>
        /// <param name="image">The captured screen image to display.</param>
        public void UpdateMagnifier(BitmapSource image)
        {
            MagnifierImage.Source = image;
        }

        private void GridCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawPixelGrid();
        }

        private void DrawPixelGrid()
        {
            GridCanvas.Children.Clear();

            double cellWidth = GridCanvas.ActualWidth / 15;
            double cellHeight = GridCanvas.ActualHeight / 15;

            // Vertical lines
            for (int i = 1; i < 15; i++)
            {
                GridCanvas.Children.Add(new Line
                {
                    X1 = i * cellWidth,
                    Y1 = 0,
                    X2 = i * cellWidth,
                    Y2 = GridCanvas.ActualHeight,
                    Stroke = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)),
                    StrokeThickness = 1
                });
            }

            // Horizontal lines
            for (int i = 1; i < 15; i++)
            {
                GridCanvas.Children.Add(new Line
                {
                    X1 = 0,
                    Y1 = i * cellHeight,
                    X2 = GridCanvas.ActualWidth,
                    Y2 = i * cellHeight,
                    Stroke = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)),
                    StrokeThickness = 1
                });
            }

            // Central pixel
            GridCanvas.Children.Add(new Rectangle
            {
                Width = cellWidth,
                Height = cellHeight,
                Stroke = Brushes.White,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            });

            Canvas.SetLeft(GridCanvas.Children[^1], 7 * cellWidth);
            Canvas.SetTop(GridCanvas.Children[^1], 7 * cellHeight);
        }

        /// <summary>
        /// Updates the magnifier image with the current screen capture.
        /// </summary>
        public void UpdateMagnifier()
        {
            MagnifierImage.Source = EyedropperService.CaptureAroundCursor();
        }

        protected override void OnClosed(EventArgs e)
        {
            timer.Stop();
            mouseHookService.Dispose();

            base.OnClosed(e);
        }

        private void UpdateWindowPosition(int cursorX, int cursorY)
        {
            EyedropperService.RECT bounds = EyedropperService.GetMonitorWorkingArea(cursorX, cursorY);

            double left = cursorX + 20;
            double top = cursorY + 20;

            if (left + ActualWidth > bounds.Right)
                left = cursorX - ActualWidth - 20;

            if (top + ActualHeight > bounds.Bottom)
                top = cursorY - ActualHeight - 20;

            left = Math.Max(bounds.Left, Math.Min(left, bounds.Right - ActualWidth));
            top = Math.Max(bounds.Top, Math.Min(top, bounds.Bottom - ActualHeight));

            Left = left;
            Top = top;
        }

        private void MouseHook_LeftButtonDown()
        {
            Dispatcher.Invoke(() =>
            {
                (int x, int y) = EyedropperService.GetCursorPosition();

                Color color = EyedropperService.GetScreenPixelColor(x, y);

                ColorPicked?.Invoke(color);

                Close();
            });
        }
    }
}
