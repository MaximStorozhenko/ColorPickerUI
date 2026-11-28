using ColorPickerUI.Enums;

namespace ColorPickerUI.Localization
{
    /// <summary>
    /// Represents a language option available in the ColorPicker control.
    /// </summary>
    public class ColorPickerLanguageOption
    {
        /// <summary>
        /// Gets the language identifier.
        /// </summary>
        public ColorPickerLanguage Language { get; }

        /// <summary>
        /// Gets the display name of the language.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPickerLanguageOption"/> class.
        /// </summary>
        public ColorPickerLanguageOption(ColorPickerLanguage language, string displayName)
        {
            Language = language;
            DisplayName = displayName;
        }
    }
}