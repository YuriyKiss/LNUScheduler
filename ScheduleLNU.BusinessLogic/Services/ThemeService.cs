using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.Constants;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.DTOs.Mappers;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;

namespace ScheduleLNU.BusinessLogic.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IRepository<Student> studentRepository;

        private readonly IRepository<Theme> themeRepository;

        private readonly ICookieService cookieService;

        private readonly ILoggingService<ThemeService> logger;

        public ThemeService(
            IRepository<Student> studentRepository,
            IRepository<Theme> themeRepository,
            ICookieService cookieService,
            ILoggingService<ThemeService> logger)
        {
            this.studentRepository = studentRepository;
            this.themeRepository = themeRepository;
            this.cookieService = cookieService;
            this.logger = logger;
        }

        public async Task<IEnumerable<ThemeDto>> GetAllAsync()
        {
            var studentId = cookieService.GetStudentId();
            var studentRecord = await studentRepository
                .SelectAsync(s => s.Id == studentId, s => s.Themes, s => s.SelectedTheme);

            if (studentRecord is null)
            {
                logger.LogError(LoggingConstants.StudentNotFound, studentId);
                return Array.Empty<ThemeDto>();
            }

            if (studentRecord.SelectedTheme is null)
            {
                logger.LogInfo(LoggingConstants.StudentHasNoSelectedTheme, studentId);
            }

            var themeDtosToReturn = studentRecord.Themes
                .Select(t => new ThemeDto()
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsSelected = t.Id == studentRecord.SelectedTheme?.Id,
                    Font = t.Font,
                    FontSize = t.FontSize,
                    ForeColor = t.ForeColor,
                    BackColor = t.BackColor,
                });

            logger.LogInfo(LoggingConstants.StudentViewedThemes, studentId, themeDtosToReturn.Count());
            return themeDtosToReturn;
        }

        public async Task<Theme> GetAsync(int themeID)
        {
            var studentId = cookieService.GetStudentId();
            var studentRecord = await studentRepository.SelectAsync(s => s.Id == studentId, s => s.Themes);

            if (studentRecord is null)
            {
                logger.LogError(LoggingConstants.StudentNotFound, studentId);
                return null;
            }

            if (studentRecord.Themes is null)
            {
                logger.LogWarning(LoggingConstants.StudentHasNoThemes, studentId);
                return null;
            }

            if (studentRecord.Themes.Count == 0)
            {
                logger.LogInfo(LoggingConstants.StudentHasNoThemes, studentId);
            }

            var theme = studentRecord.Themes.FirstOrDefault(t => t.Id == themeID);
            if (theme is null)
            {
                logger.LogWarning(LoggingConstants.StudentHasNoTheme, studentId, themeID);
            }

            logger.LogInfo(LoggingConstants.StudentViewedTheme, studentId, themeID);
            return theme;
        }

        public async Task AddAsync(Theme theme)
        {
            var studentId = cookieService.GetStudentId();
            var studentRecord = await studentRepository.SelectAsync(s => s.Id.Equals(studentId), s => s.Themes);

            if (studentRecord is null)
            {
                logger.LogError(LoggingConstants.StudentNotFound, studentId);
                return;
            }

            if (theme.Id < 0)
            {
                logger.LogError(LoggingConstants.StudentHasInvalidTheme, studentId, theme.Id);
                return;
            }

            if (studentRecord.Themes is null)
            {
                logger.LogWarning(LoggingConstants.StudentHasNoThemes, studentId);
                return;
            }

            if (studentRecord.Themes.Count == 0)
            {
                logger.LogInfo(LoggingConstants.StudentHasNoThemes, studentId);
            }

            studentRecord.Themes.Add(theme);
            await studentRepository.UpdateAsync(studentRecord);
        }

        public async Task EditAsync(ThemeDto themeDto)
        {
            var studentId = cookieService.GetStudentId();
            if (themeDto.IsSelected)
            {
                cookieService.SetSessionData(
                 (ThemeConstants.FontSizeKey, themeDto.FontSize),
                 (ThemeConstants.FontFamilyKey, themeDto.Font),
                 (ThemeConstants.BackColorKey, themeDto.BackColor),
                 (ThemeConstants.ForeColorKey, themeDto.ForeColor));

                logger.LogInfo(LoggingConstants.SelectedThemeWasChanded, studentId, themeDto.Id);
            }

            logger.LogInfo(LoggingConstants.StudentChangedTheme, studentId, themeDto.Id);
            await themeRepository.UpdateAsync(themeDto.ToTheme());
        }

        public async Task DeleteAsync(Theme theme)
        {
            var studentId = cookieService.GetStudentId();
            if (theme.Id < 0)
            {
                logger.LogError(LoggingConstants.StudentHasInvalidTheme, studentId, theme.Id);
                return;
            }

            logger.LogInfo(LoggingConstants.StudentDeletesTheme, studentId, theme.Id);
            await themeRepository.DeleteAsync(theme);
        }

        public async Task SelectAsync(Theme theme)
        {
            var studentId = cookieService.GetStudentId();
            var studentRecord = await studentRepository.SelectAsync(s => s.Id.Equals(studentId), s => s.SelectedTheme);

            if (studentRecord is null)
            {
                logger.LogError(LoggingConstants.StudentNotFound, studentId);
                return;
            }

            studentRecord.SelectedTheme = theme;

            cookieService.SetSessionData(
                 (ThemeConstants.FontSizeKey, theme.FontSize),
                 (ThemeConstants.FontFamilyKey, theme.Font),
                 (ThemeConstants.BackColorKey, theme.BackColor),
                 (ThemeConstants.ForeColorKey, theme.ForeColor));

            logger.LogInfo(LoggingConstants.StudentSelectsTheme, studentRecord.Id, theme.Id);
            await studentRepository.UpdateAsync(studentRecord);
        }

        public async Task DeselectAsync()
        {
            var studentId = cookieService.GetStudentId();
            var studentRecord = await studentRepository.SelectAsync(s => s.Id.Equals(studentId), s => s.SelectedTheme);

            if (studentRecord is null)
            {
                logger.LogError(LoggingConstants.StudentNotFound, studentId);
                return;
            }

            cookieService.SetSessionData(
                 (ThemeConstants.FontSizeKey, ThemeConstants.DefaultTheme.FontSize),
                 (ThemeConstants.FontFamilyKey, ThemeConstants.DefaultTheme.Font),
                 (ThemeConstants.BackColorKey, ThemeConstants.DefaultTheme.BackColor),
                 (ThemeConstants.ForeColorKey, ThemeConstants.DefaultTheme.ForeColor));

            logger.LogInfo(LoggingConstants.StudentDeselectsTheme, studentId, studentRecord.SelectedTheme.Id);
            await studentRepository.SetNullAsync(studentRecord, nameof(studentRecord.SelectedTheme));
        }
    }
}
