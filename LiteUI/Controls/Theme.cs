using System.Windows.Media;

namespace LiteUI.Controls
{
    public sealed class Theme
    {
        public static readonly Theme Light = new();

        public static readonly Theme Dark = new()
        {
            Active = Color.FromRgb(0xFF, 0xFF, 0xFF),
            Inactive = Color.FromRgb(0x7F, 0x7F, 0x7F),
            Background = Color.FromRgb(0x00, 0x00, 0x00),
            AccentForeground = Color.FromRgb(0xFF, 0xFF, 0xFF),
            AccentBackground = Color.FromRgb(0x00, 0x96, 0xF3)
        };

        public Color Active { get; set; } = Color.FromRgb(0x26, 0x26, 0x26);
        public Color Inactive { get; set; } = Color.FromRgb(0x7F, 0x7F, 0x7F);
        public Color Background { get; set; } = Color.FromRgb(0xFF, 0xFF, 0xFF);
        public Color AccentForeground { get; set; } = Color.FromRgb(0xFF, 0xFF, 0xFF);
        public Color AccentBackground { get; set; } = Color.FromRgb(0x00, 0x7A, 0xCC);
    }
}
