using ColorPicker.Wpf.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
                if (selectedColor == value)
                    return;

                selectedColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedColorHex));
            }
        }

        public string SelectedColorHex =>
            $"#{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RelayCommand<string> SetColorCommand { get; }

        public MainViewModel()
        {
            SetColorCommand = new RelayCommand<string>(SetColor);
        }

        private void SetColor(string? strColor)
        {
            if (strColor is not string hex)
                return;

            //if (ColorHelper.TryParseHex(hex, out Color color))
            //    SelectedColor = color;
        }
    }
}
