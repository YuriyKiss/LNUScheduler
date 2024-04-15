using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.DataAccess.Entities;

namespace ScheduleLNU.BusinessLogic.Constants
{
    public static class ThemeConstants
    {
        private static readonly Theme DefaultThemeValue = new ()
        {
            Title = "New Theme",
            FontSize = "1.5vw",
            Font = "input-mono-narrow, monospace",
            BackColor = "#73C2FB",
            ForeColor = "#000"
        };

        public static string FontSizeKey => nameof(DefaultTheme.FontSize);

        public static string FontFamilyKey => nameof(DefaultTheme.Font);

        public static string BackColorKey => nameof(DefaultTheme.BackColor);

        public static string ForeColorKey => nameof(DefaultTheme.ForeColor);

        public static Theme DefaultTheme => DefaultThemeValue;

        public static EventStyleDto DefaultEventStyle => new ()
        {
            Id = 0,
            Title = "Default",
            BackColor = "#FFF",
            ForeColor = "#000"
        };
    }
}
