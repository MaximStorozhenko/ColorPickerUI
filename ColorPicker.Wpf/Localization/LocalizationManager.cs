using ColorPickerUI.Enums;
using System.ComponentModel;
using System.Globalization;

namespace ColorPickerUI.Localization
{
    /// <summary>
    /// Provides localization services for the ColorPicker control.
    /// </summary>
    public class LocalizationManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private CultureInfo culture = new("en");

        /// <summary>
        /// Gets or sets the current localization culture.
        /// </summary>
        public CultureInfo Culture
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
        /// Changes the localization culture and refreshes all localized strings.
        /// </summary>
        /// <param name="culture">The culture to apply.</param>
        public void ChangeCulture(CultureInfo culture)
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

        /// <summary>
        /// Gets the localized Apply button text.
        /// </summary>
        public string Apply_Text => GetString(nameof(Strings.Apply));

        /// <summary>
        /// Gets the localized Apply button's tooltip text.
        /// </summary>
        public string ApplyToolTip_Text => GetString(nameof(Strings.ApplyColor));

        /// <summary>
        /// Gets the localized Copy button's tooltip text.
        /// </summary>
        public string Copy_Text => GetString(nameof(Strings.Copy));

        /// <summary>
        /// Gets the localized Eyedropper button's tooltip text.
        /// </summary>
        public string Eyedropper_Text => GetString(nameof(Strings.Eyedropper));

        /// <summary>
        /// Gets the localized PresetColors title text.
        /// </summary>
        public string PresetColors_Text => GetString(nameof(Strings.PresetColors)).ToUpper();

        /// <summary>
        /// Gets the culture corresponding to the specified language.
        /// </summary>
        public static CultureInfo GetCulture(ColorPickerLanguage language)
        {
            return language switch
            {
                ColorPickerLanguage.English => new CultureInfo("en"),
                ColorPickerLanguage.Russian => new CultureInfo("ru"),
                ColorPickerLanguage.Ukrainian => new CultureInfo("uk"),

                ColorPickerLanguage.Auto => GetApplicationCulture(),

                _ => new CultureInfo("en")
            };
        }

        /// <summary>
        /// Determines the supported culture based on the application's current UI culture.
        /// </summary>
        /// <returns>The matching supported culture, or English if the application language is unsupported.</returns>
        private static CultureInfo GetApplicationCulture()
        {
            string language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            return language switch
            {
                "ru" => new CultureInfo("ru"),
                "uk" => new CultureInfo("uk"),
                _ => new CultureInfo("en")
            };
        }
    }
}
