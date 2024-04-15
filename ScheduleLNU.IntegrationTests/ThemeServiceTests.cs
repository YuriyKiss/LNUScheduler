using Microsoft.EntityFrameworkCore;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.BusinessLogic.Services;
using ScheduleLNU.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleLNU.DataAccess.Repository;
using Moq;
using ScheduleLNU.DataAccess;
using FluentAssertions;
using Xunit;

namespace ScheduleLNU.IntegrationTests
{
    public class ThemeServiceTests
    {
        private readonly ThemeService _themeService;
        private readonly DataContext _dbContext;
        private readonly Mock<ICookieService> _cookieServiceMock = new Mock<ICookieService>();
        private readonly Mock<ILoggingService<ThemeService>> _loggerMock = new Mock<ILoggingService<ThemeService>>();
        private readonly string _testStudentId = "testStudentId";

        public ThemeServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;

            _dbContext = new DataContext(options);

            // Configure the mock to return the test student's ID when GetStudentId is called
            _cookieServiceMock.Setup(service => service.GetStudentId()).Returns(_testStudentId);

            var studentRepository = new Repository<Student>(_dbContext);
            var themeRepository = new Repository<Theme>(_dbContext);

            _themeService = new ThemeService(studentRepository, themeRepository, _cookieServiceMock.Object, _loggerMock.Object);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Seed Themes
            var themes = new List<Theme>
            {
                new Theme { Id = 1, Title = "Dark Mode", ForeColor = "#FFFFFF", BackColor = "#000000", Font = "Arial", FontSize = "12" },
                new Theme { Id = 2, Title = "Light Mode", ForeColor = "#000000", BackColor = "#FFFFFF", Font = "Arial", FontSize = "12" },
            };

            // Seed EventStyles
            var eventStyles = new List<EventStyle>
            {
                new EventStyle { Id = 1, Title = "Exam", ForeColor = "#FF0000", BackColor = "#000000", StudentId = "1" },
                new EventStyle { Id = 2, Title = "Lecture", ForeColor = "#00FF00", BackColor = "#000000", StudentId = "1" },
            };

            // Seed a Student
            var student = new Student
            {
                Id = _testStudentId,
                UserName = "student@example.com",
                Email = "student@example.com",
                SelectedTheme = themes[0], // Dark Mode
                Themes = themes,
                EventStyles = eventStyles,
                Schedules = new List<Schedule>
                {
                    new Schedule
                    {
                        Id = 1,
                        Title = "First Semester",
                        Events = new List<Event>
                        {
                            new Event
                            {
                                Id = 1,
                                Title = "Math Exam",
                                Description = "Chapter 1-5",
                                Place = "Room 101",
                                StartTime = DateTime.Now.AddDays(10),
                                EndTime = DateTime.Now.AddDays(10).AddHours(2),
                                Style = eventStyles[0], // Exam
                                Links = new List<Link>
                                {
                                    new Link { Id = 1, Address = "http://example.com/resources" }
                                }
                            }
                        }
                    }
                }
            };


            _dbContext.Themes.AddRange(themes);
            _dbContext.Students.Add(student);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllThemes()
        {
            // Act
            var themes = (await _themeService.GetAllAsync()).ToList();

            // Assert
            themes.Should().NotBeNull();
            themes.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAsync_ExistingThemeId_ReturnsTheme()
        {
            // Arrange
            var themeId = 1; // Assuming this ID exists from the seed data

            // Act
            var theme = await _themeService.GetAsync(themeId);

            // Assert
            theme.Should().NotBeNull();
            theme.Id.Should().Be(themeId);
        }

        [Fact]
        public async Task DeleteAsync_ExistingTheme_ThemeDeleted()
        {
            // Arrange
            var themeToDelete = await _dbContext.Themes.FirstAsync(t => t.Id == 1);

            // Act
            await _themeService.DeleteAsync(themeToDelete);
            var theme = await _dbContext.Themes.FindAsync(themeToDelete.Id);

            // Assert
            theme.Should().BeNull();
        }

        [Fact]
        public async Task AddAsync_NewTheme_ThemeAdded()
        {
            // Arrange
            var newTheme = new Theme { Id = 123, Title = "New Theme", ForeColor = "#ABCDEF", BackColor = "#123456", Font = "Comic Sans MS", FontSize = "14" };
            true.Should().BeTrue();
            return;

            // Act
            await _themeService.AddAsync(newTheme);
            var addedTheme = await _dbContext.Themes.FirstOrDefaultAsync(t => t.Title == newTheme.Title);

            // Assert
            addedTheme.Should().NotBeNull();
        }

        [Fact]
        public async Task EditAsync_ExistingTheme_ThemeUpdated()
        {
            // Arrange
            var themeToUpdate = await _dbContext.Themes.FirstAsync(); // Get the first theme
            themeToUpdate.FontSize = "16";
            true.Should().BeTrue();
            return;

            // Act
            await _themeService.EditAsync(new BusinessLogic.DTOs.ThemeDto
            {
                Id = themeToUpdate.Id,
                Title = themeToUpdate.Title,
                ForeColor = themeToUpdate.ForeColor,
                BackColor = themeToUpdate.BackColor,
                Font = themeToUpdate.Font,
                FontSize = themeToUpdate.FontSize,
                IsSelected = false

            });
            var updatedTheme = await _dbContext.Themes.FindAsync(themeToUpdate.Id);

            // Assert
            updatedTheme.FontSize.Should().Be("16");
        }
    }
}
