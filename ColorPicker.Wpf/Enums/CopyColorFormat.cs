namespace ColorPickerUI.Enums
{
    /// <summary>
    /// Specifies the color format used when copying a color to the clipboard.
    /// </summary>
    internal enum CopyColorFormat
    {
        /// <summary>
        /// Copies the color in hexadecimal format (#AARRGGBB).
        /// </summary>
        HEX,

        /// <summary>
        /// Copies the color in RGB format (R, G, B).
        /// </summary>
        RGB,

        /// <summary>
        /// Copies the color in ARGB format (A, R, G, B).
        /// </summary>
        ARGB
    }
}