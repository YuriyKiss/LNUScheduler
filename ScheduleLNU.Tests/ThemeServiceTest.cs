using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using ScheduleLNU.BusinessLogic.Constants;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.DTOs.Comparers;
using ScheduleLNU.BusinessLogic.DTOs.Mappers;
using ScheduleLNU.BusinessLogic.Services;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;
using Xunit;

namespace ScheduleLNU.Tests.BusinessLogic
{
    public class ThemeServiceTest
    {
        private readonly ThemeService themeStyleService;

        private readonly Mock<IRepository<Student>> studentRepositoryMock = new ();
        
        private readonly Mock<IRepository<Theme>> themeRepositoryMock = new ();

        private readonly Mock<ICookieService> cookieServiceMock = new ();
        
        private readonly Mock<ILoggingService<ThemeService>> loggerMock = new ();

        public ThemeServiceTest()
        {
            themeStyleService = new ThemeService(
                studentRepositoryMock.Object,
                themeRepositoryMock.Object,
                cookieServiceMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyArray_WhenStudentIsNotExists()
        {
            var studentId = It.IsAny<string>();
            var expectedResult = Array.Empty<ThemeDto>();
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(
                s => s.Id == studentId,
                s => s.Themes,
                s => s.SelectedTheme))
                .ReturnsAsync(() => null);

            var actualResult = await themeStyleService.GetAllAsync();

            Assert.Equal(expectedResult, actualResult);
            loggerMock.Verify(l => l.LogError(LoggingConstants.StudentNotFound, studentId));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnThemeDtoList_WhenStudentExistsWithoutSelectedTheme()
        {
            var studentId = "proper-student-id";
            var expectedResult = new List<ThemeDto>() { new () { Id = 104 }, new () { Id = 102 }, new () { Id = 105 } };

            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(
                s => s.Id == studentId,
                s => s.Themes,
                s => s.SelectedTheme))
                .ReturnsAsync(new Student()
                {
                    Id = studentId,
                    Themes = expectedResult.Select(s => s.ToTheme()).ToList()
                });

            var actualResult = await themeStyleService.GetAllAsync();

            Assert.Equal(expectedResult.Count, actualResult.Count());
            Assert.All(
                expectedResult.Zip(actualResult),
                t => Assert.True(ThemeDtoComparer.Equal(t.First, t.Second)));

            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentViewedThemes, studentId, expectedResult.Count));
            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentHasNoSelectedTheme, studentId));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProperList_WhenStudentExistsWithSelectedTheme()
        {
            var studentId = "proper-student-id";
            var expectedResult = new List<ThemeDto>()
            {
                new () { Id = 104 }, new () { Id = 102, IsSelected = true }, new () { Id = 105 }
            };

            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(
                s => s.Id == studentId,
                s => s.Themes,
                s => s.SelectedTheme))
                .ReturnsAsync(new Student()
                {
                    Id = studentId,
                    Themes = expectedResult.Select(s => s.ToTheme()).ToList(),
                    SelectedTheme = new () { Id = 102 }
                });

            var acutalResult = await themeStyleService.GetAllAsync();

            Assert.Equal(expectedResult.Count, acutalResult.Count());
            Assert.All(
                expectedResult.Zip(acutalResult),
                t => Assert.True(ThemeDtoComparer.Equal(t.First, t.Second)));

            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentViewedThemes, studentId, expectedResult.Count));
            loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenStudentNotExists()
        {
            var studentId = "some-student-id";
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id == studentId, s => s.Themes))
                .ReturnsAsync(() => null);

            var actualResult = await themeStyleService.GetAsync(It.IsAny<int>());

            Assert.Null(actualResult);
            loggerMock.Verify(l => l.LogError(LoggingConstants.StudentNotFound, studentId));
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenStudentExistsWithNullThemes()
        {
            var studentId = "some-student-id";
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id == studentId, s => s.Themes))
                .ReturnsAsync(new Student()
                {
                    Id = studentId
                });

            var actualResult = await themeStyleService.GetAsync(It.IsAny<int>());

            Assert.Null(actualResult);
            loggerMock.Verify(l => l.LogWarning(LoggingConstants.StudentHasNoThemes, studentId));
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenStudentExistsWithZeroThemes()
        {
            var themeId = 2;
            var studentId = "some-student-id";
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id == studentId, s => s.Themes))
                .ReturnsAsync(new Student()
                {
                    Id = studentId,
                    Themes = new () { }
                });

            var actualResult = await themeStyleService.GetAsync(themeId);

            Assert.Null(actualResult);
            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentHasNoThemes, studentId));
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenStudentExistsWithoutExpectedTheme()
        {
            var themeId = 2;
            var studentId = "some-student-id";
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id == studentId, s => s.Themes))
                .ReturnsAsync(new Student()
                {
                    Id = studentId,
                    Themes = new () { new () { Id = 228 }, new () { Id = 512 } }
                });

            var actualResult = await themeStyleService.GetAsync(themeId);

            Assert.Null(actualResult);
            loggerMock.Verify(l => l.LogWarning(LoggingConstants.StudentHasNoTheme, studentId, themeId));
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenStudentExistsWithExpectedTheme()
        {
            var themeId = 228;
            var studentId = "some-student-id";
            var expectableResult = new Theme() { Id = 228 };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id == studentId, s => s.Themes))
                .ReturnsAsync(new Student()
                {
                    Id = studentId,
                    Themes = new () { expectableResult, new () { Id = 512 } }
                });

            var actualResult = await themeStyleService.GetAsync(themeId);

            Assert.Equal(expectableResult.Id, actualResult.Id);
            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentViewedTheme, studentId, themeId));
        }

        [Fact]
        public async Task EditAsync_ShouldLogChangeTheme_WhenThemeExists()
        {
            var studentId = It.IsAny<string>();
            var themeId = 10;
            var themeDto = new ThemeDto() { Id = themeId, BackColor = "#fff" };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);

            await themeStyleService.EditAsync(themeDto);

            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentChangedTheme, studentId, themeDto.Id));
            loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task EditAsync_ShouldLogChangeSelectedTheme_WhenSelectedThemeChanded()
        {
            var studentId = It.IsAny<string>();
            var themeId = 10;
            var themeDto = new ThemeDto() { Id = themeId, BackColor = "#fff", IsSelected = true };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);

            await themeStyleService.EditAsync(themeDto);

            loggerMock.Verify(l => l.LogInfo(LoggingConstants.SelectedThemeWasChanded, studentId, themeDto.Id));
        }

        [Fact]
        public async Task AddAsync_ShouldLogError_WhenStudentNotExists()
        {
            var studentId = It.IsAny<string>();
            var themeId = 10;
            var theme = new Theme() { Id = themeId, BackColor = "#fff" };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);

            await themeStyleService.AddAsync(theme);

            loggerMock.Verify(l => l.LogError(LoggingConstants.StudentNotFound, studentId));
        }

        [Fact]
        public async Task AddAsync_ShouldLogError_WhenIdLessThanZero()
        {
            var studentId = It.IsAny<string>();
            var student = new Student() { Id = studentId, Themes = null };
            var themeId = -10;
            var theme = new Theme() { Id = themeId, BackColor = "#fff" };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id.Equals(studentId), s => s.Themes))
                .ReturnsAsync(student);

            await themeStyleService.AddAsync(theme);

            loggerMock.Verify(l => l.LogError(LoggingConstants.StudentHasInvalidTheme, studentId, themeId));
        }

        [Fact]
        public async Task AddAsync_ShouldLogWarning_WhenThemeIsNull()
        {
            var studentId = It.IsAny<string>();
            var student = new Student() { Id = studentId, Themes = null };
            var themeId = 10;
            var theme = new Theme() { Id = themeId, BackColor = "#fff" };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id.Equals(studentId), s => s.Themes))
                .ReturnsAsync(student);

            await themeStyleService.AddAsync(theme);

            loggerMock.Verify(l => l.LogWarning(LoggingConstants.StudentHasNoThemes, studentId));
        }

        [Fact]
        public async Task AddAsync_ShouldLogInfo_WhenThemeCountIsZero()
        {
            var studentId = It.IsAny<string>();
            var student = new Student() { Id = studentId, Themes = new List<Theme>() };
            var themeId = 10;
            var theme = new Theme() { Id = themeId, BackColor = "#fff" };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id.Equals(studentId), s => s.Themes))
                .ReturnsAsync(student);

            await themeStyleService.AddAsync(theme);

            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentHasNoThemes, studentId));
        }

        [Fact]
        public async Task DeleteAsync_ShouldLogError_WhenInvalidTheme()
        {
            var studentId = It.IsAny<string>();
            var themeId = -1;
            var theme = new Theme() { Id = themeId };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);

            await themeStyleService.DeleteAsync(theme);

            loggerMock.Verify(l => l.LogError(LoggingConstants.StudentHasInvalidTheme, studentId, themeId));
        }

        [Fact]
        public async Task DeleteAsync_ShouldLogInfo_WhenProperTheme()
        {
            var studentId = It.IsAny<string>();
            var themeId = 1;
            var theme = new Theme() { Id = themeId };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);

            await themeStyleService.DeleteAsync(theme);

            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentDeletesTheme, studentId, themeId));
        }

        [Fact]
        public async Task SelectAsync_ShouldLogError_WhenStudentNotExists()
        {
            var studentId = It.IsAny<string>();
            var themeId = 1;
            var theme = new Theme() { Id = themeId };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id.Equals(studentId), s => s.SelectedTheme))
                .ReturnsAsync(() => null);

            await themeStyleService.SelectAsync(theme);

            loggerMock.Verify(l => l.LogError(LoggingConstants.StudentNotFound, studentId));
        }

        [Fact]
        public async Task SelectAsync_ShouldLogInfo_WhenStudentExists()
        {
            var studentId = It.IsAny<string>();
            var themeId = 1;
            var theme = new Theme() { Id = themeId };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id.Equals(studentId), s => s.SelectedTheme))
                .ReturnsAsync(new Student() { Id = studentId });

            await themeStyleService.SelectAsync(theme);

            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentSelectsTheme, studentId, themeId));
        }

        [Fact]
        public async Task DeselectAsync_ShouldLogError_WhenStudentNotExists()
        {
            var studentId = It.IsAny<string>();
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id.Equals(studentId), s => s.SelectedTheme))
                .ReturnsAsync(() => null);

            await themeStyleService.DeselectAsync();

            loggerMock.Verify(l => l.LogError(LoggingConstants.StudentNotFound, studentId));
        }

        [Fact]
        public async Task DeselectAsync_ShouldLogInfo_WhenStudentExists()
        {
            var studentId = It.IsAny<string>();
            var themeId = 1;
            var theme = new Theme() { Id = themeId };
            cookieServiceMock.Setup(x => x.GetStudentId()).Returns(studentId);
            studentRepositoryMock.Setup(x => x.SelectAsync(s => s.Id.Equals(studentId), s => s.SelectedTheme))
                .ReturnsAsync(new Student() { Id = studentId, SelectedTheme = theme });

            await themeStyleService.DeselectAsync();

            loggerMock.Verify(l => l.LogInfo(LoggingConstants.StudentDeselectsTheme, studentId, themeId));
        }
    }
}
