using ColorPickerUI.Enums;
using System.ComponentModel;
using System.Globalization;

namespace ColorPickerUI.Localization
{
    /// <summary>
    /// Manages localized resources for a ColorPicker instance.
    /// </summary>
    internal class LocalizationManager : IColorPickerLocalization
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Contains the cultures supported by the ColorPicker control.
        /// </summary>
        private static readonly Dictionary<ColorPickerLanguage, CultureInfo> SupportedLanguages = new()
        {
            [ColorPickerLanguage.English] = new("en"),
            [ColorPickerLanguage.Russian] = new("ru"),
            [ColorPickerLanguage.Ukrainian] = new("uk")
        };

        private CultureInfo culture = new("en");

        /// <summary>
        /// Gets or sets the current localization culture.
        /// </summary>
        internal CultureInfo Culture
        {
            get => culture;
            private set
            {
                if (culture.Equals(value)) return;

                culture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
        }

        /// <summary>
        /// Gets the automatically generated list of available language options.
        /// </summary>
        public static IReadOnlyList<ColorPickerLanguageOption> AvailableLanguages { get; } = new[]
            {
                new ColorPickerLanguageOption(ColorPickerLanguage.Auto, "Auto")
            }
        .Concat(SupportedLanguages.Select(language =>
        new ColorPickerLanguageOption(language.Key, GetNativeLanguageName(language.Value)))).ToArray();

        /// <summary>
        /// Updates the localization culture.
        /// </summary>
        /// <param name="culture">The culture to apply.</param>
        internal void ChangeCulture(CultureInfo culture)
        {
            Culture = culture;
        }

        /// <summary>
        /// Retrieves a localized string using the specified resource key
        /// and the current localization culture.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <returns>The localized string, or the resource key if no translation is found.</returns>
        private string GetString(string key)
        {
            return Strings.ResourceManager.GetString(key, Culture) ?? key;
        }

        public string Apply_Text => GetString(nameof(Strings.Apply));
        public string ApplyToolTip_Text => GetString(nameof(Strings.ApplyColor));
        public string Copy_Text => GetString(nameof(Strings.Copy));
        public string Eyedropper_Text => GetString(nameof(Strings.Eyedropper));
        public string PresetColors_Text => GetString(nameof(Strings.PresetColors)).ToUpper();

        /// <summary>
        /// Gets the culture associated with the specified language.
        /// </summary>
        /// <param name="language">The selected ColorPicker language.</param>
        /// <returns>The culture used for localization.</returns>
        internal static CultureInfo GetCulture(ColorPickerLanguage language)
        {
            if(language.Equals(ColorPickerLanguage.Auto))
            {
                string currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                return SupportedLanguages.Values.FirstOrDefault(
                    Culture => Culture.TwoLetterISOLanguageName == currentLanguage)
                    ?? SupportedLanguages[ColorPickerLanguage.English];
            }

            return SupportedLanguages.TryGetValue(language, out CultureInfo? culture)
                ? culture : SupportedLanguages[ColorPickerLanguage.English];
        }

        /// <summary>
        /// Gets the native display name of the specified culture.
        /// </summary>
        /// <param name="culture">The culture to retrieve the native name for.</param>
        /// <returns>The capitalized native name of the culture.</returns>
        private static string GetNativeLanguageName(CultureInfo culture)
        {
            return culture.TextInfo.ToTitleCase(culture.NativeName);
        }
    }
}