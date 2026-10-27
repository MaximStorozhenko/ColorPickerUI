using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

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
    }
}
