using System.ComponentModel;

namespace ColorPickerUI.Localization
{
    /// <summary>
    /// Provides localized text displayed by the ColorPicker control.
    /// </summary>
    /// <remarks>
    /// This interface is used internally by the control's XAML bindings
    /// to display text in the selected language.
    /// </remarks>
    public interface IColorPickerLocalization : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the localized Apply button text.
        /// </summary>
        string Apply_Text { get; }

        /// <summary>
        /// Gets the localized Apply button's tooltip text.
        /// </summary>
        string ApplyToolTip_Text { get; }

        /// <summary>
        /// Gets the localized Copy button's tooltip text.
        /// </summary>
        string Copy_Text { get; }

        /// <summary>
        /// Gets the localized Eyedropper button's tooltip text.
        /// </summary>
        string Eyedropper_Text { get; }

        /// <summary>
        /// Gets the localized PresetColors title text.
        /// </summary>
        string PresetColors_Text { get; }
    }
}