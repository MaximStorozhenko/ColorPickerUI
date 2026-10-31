namespace ColorPickerUI.Enums
{
    /// <summary>
    /// Specifies the preset color palette used by the color picker.
    /// </summary>
    public enum ColorPaletteType
    {
        /// <summary>
        /// Uses the classic set of preset colors.
        /// </summary>
        Classic,

        /// <summary>
        /// Uses colors inspired by Material Design.
        /// </summary>
        Material,

        /// <summary>
        /// Uses soft pastel colors.
        /// </summary>
        Pastel,

        /// <summary>
        /// Uses different shades of gray.
        /// </summary>
        Grayscale,

        /// <summary>
        /// Uses an extended set of preset colors.
        /// </summary>
        Extended,

        /// <summary>
        /// Uses custom colors provided through <see cref="ColorPicker.PresetCustomColors"/>.
        /// </summary>
        Custom
    }
}