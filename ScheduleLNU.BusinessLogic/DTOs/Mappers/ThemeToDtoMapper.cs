using ScheduleLNU.DataAccess.Entities;

namespace ScheduleLNU.BusinessLogic.DTOs.Mappers
{
    public static class ThemeToDtoMapper
    {
        public static Theme ToTheme(this ThemeDto themeDto)
        {
            return new Theme()
            {
                Id = themeDto.Id,
                Title = themeDto.Title,
                ForeColor = themeDto.ForeColor,
                BackColor = themeDto.BackColor,
                Font = themeDto.Font,
                FontSize = themeDto.FontSize
            };
        }
    }
}
