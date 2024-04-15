using Microsoft.EntityFrameworkCore;
using Moq;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;
using ScheduleLNU.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleLNU.BusinessLogic.Services;
using ScheduleLNU.BusinessLogic.DTOs;
using Xunit;
using FluentAssertions;

namespace ScheduleLNU.IntegrationTests
{
    public class EventStyleServiceTests
    {
        private readonly EventStyleService _eventStyleService;
        private readonly DataContext _dbContext;
        private readonly Mock<ICookieService> _cookieServiceMock = new Mock<ICookieService>();
        private readonly string _testStudentId = "testStudentId";

        public EventStyleServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new DataContext(options);
            _cookieServiceMock.Setup(service => service.GetStudentId()).Returns(_testStudentId);

            var eventStyleRepository = new Repository<EventStyle>(_dbContext);
            _eventStyleService = new EventStyleService(eventStyleRepository, _cookieServiceMock.Object);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var eventStyles = new List<EventStyle>
            {
                new EventStyle { Title = "Lecture", ForeColor = "#00FF00", BackColor = "#000000", StudentId = _testStudentId },
                new EventStyle { Title = "Exam", ForeColor = "#FF0000", BackColor = "#000000", StudentId = _testStudentId }
            };

            _dbContext.EventsStyles.AddRange(eventStyles);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task AddAsync_NewEventStyle_EventStyleAdded()
        {
            var newEventStyleDto = new EventStyleDto { Title = "Study Group", ForeColor = "#0000FF", BackColor = "#FFFFFF" };

            await _eventStyleService.AddAsync(newEventStyleDto);

            var addedEventStyle = _dbContext.EventsStyles.FirstOrDefault(es => es.Title == newEventStyleDto.Title && es.StudentId == _testStudentId);
            addedEventStyle.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAsync_ExistingEventStyle_EventStyleDeleted()
        {
            var existingEventStyle = _dbContext.EventsStyles.First();
            await _eventStyleService.DeleteAsync(existingEventStyle.Id);

            var deletedEventStyle = await _dbContext.EventsStyles.FindAsync(existingEventStyle.Id);
            deletedEventStyle.Should().BeNull();
        }

        [Fact]
        public async Task EditAsync_ExistingEventStyle_EventStyleUpdated()
        {
            var existingEventStyle = _dbContext.EventsStyles.First();
            var updatedEventStyleDto = new EventStyleDto
            {
                Id = existingEventStyle.Id,
                Title = "Updated Title",
                ForeColor = "#ABCDEF",
                BackColor = "#FEDCBA"
            };

            var success = await _eventStyleService.EditAsync(updatedEventStyleDto);

            var updatedEventStyle = await _dbContext.EventsStyles.FindAsync(existingEventStyle.Id);
            success.Should().BeTrue();
            updatedEventStyle.Title.Should().Be(updatedEventStyleDto.Title);
            updatedEventStyle.ForeColor.Should().Be(updatedEventStyleDto.ForeColor);
            updatedEventStyle.BackColor.Should().Be(updatedEventStyleDto.BackColor);
        }
    }
}
