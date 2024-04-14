using FluentAssertions;
using MailKit.Net.Smtp;
using MimeKit;
using NSubstitute;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services;
using ScheduleLNU.BusinessLogic.Services.EmailService;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleLNU.Tests
{
    public class EventServiceTests
    {
        private readonly IRepository<Schedule> _scheduleRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<EventStyle> _eventStyleRepository;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _scheduleRepository = Substitute.For<IRepository<Schedule>>();
            _eventRepository = Substitute.For<IRepository<Event>>();
            _eventStyleRepository = Substitute.For<IRepository<EventStyle>>();
            _eventService = new EventService(_scheduleRepository, _eventRepository, _eventStyleRepository);
        }

        [Fact]
        public async Task GetEventsDataAsync_WhenScheduleExists_ShouldReturnScheduleTitleAndEvents()
        {
            // Arrange
            var scheduleId = 1;
            var scheduleRecord = new Schedule
            {
                Id = scheduleId,
                Title = "Test Schedule",
                Events = new List<Event>
                {
                    new Event { Id = 1, Title = "Event 1" },
                    new Event { Id = 2, Title = "Event 2" }
                }
            };
            _scheduleRepository.SelectAsync(Arg.Any<Expression<Func<Schedule, bool>>>(), Arg.Any<Expression<Func<Schedule, object>>[]>())
                .Returns(Task.FromResult(scheduleRecord));
            _eventRepository.SelectAllAsync(Arg.Any<Expression<Func<Event, bool>>>(), Arg.Any<Expression<Func<Event, object>>[]>())
                .Returns(Task.FromResult<IEnumerable<Event>>(scheduleRecord.Events));

            // Act
            var (scheduleTitle, events) = await _eventService.GetEventsDataAsync(scheduleId);

            // Assert
            scheduleTitle.Should().Be("Test Schedule");
            events.Should().NotBeNull().And.HaveCount(2);
            events.Should().Contain(e => e.Title == "Event 1");
            events.Should().Contain(e => e.Title == "Event 2");
        }

        [Fact]
        public async Task GetEventsDataAsync_WhenScheduleDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var scheduleId = 1;
            _scheduleRepository.SelectAsync(Arg.Any<Expression<Func<Schedule, bool>>>(), Arg.Any<Expression<Func<Schedule, object>>[]>())
                .Returns(Task.FromResult<Schedule>(null));

            // Act
            var (scheduleTitle, events) = await _eventService.GetEventsDataAsync(scheduleId);

            // Assert
            scheduleTitle.Should().BeNull();
            events.Should().NotBeNull().And.BeEmpty();

        }

        [Fact]
        public async Task AddAsync_ShouldAddEventToSchedule()
        {
            // Arrange
            var eventDto = new EventDto
            {
                ScheduleId = 1,
                Title = "New Event",
                Description = "Description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Place = "Some Place",
                StyleId = 1 // Assuming valid style ID
            };
            var scheduleRecord = new Schedule { Id = eventDto.ScheduleId, Events = new List<Event>() };
            _scheduleRepository.SelectAsync(Arg.Any<Expression<Func<Schedule, bool>>>(), Arg.Any<Expression<Func<Schedule, object>>[]>())
                .Returns(Task.FromResult(scheduleRecord));
            _eventStyleRepository.SelectAsync(Arg.Any<Expression<Func<EventStyle, bool>>>()).Returns(Task.FromResult(new EventStyle()));

            // Act
            await _eventService.AddAsync(eventDto);

            // Assert
            await _scheduleRepository.Received(1).UpdateAsync(scheduleRecord);
            scheduleRecord.Events.Should().ContainSingle(e => e.Title == "New Event");
        }

        [Fact]
        public async Task EditAsync_ShouldEditEvent()
        {
            // Arrange
            var eventDto = new EventDto
            {
                Id = 1,
                Title = "Edited Event",
                Description = "Edited Description",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2),
                Place = "Edited Place",
                StyleId = 1 // Assuming valid style ID
            };
            var eventRecord = new Event { Id = eventDto.Id, Title = "Existing Event" };
            _eventStyleRepository.SelectAsync(Arg.Any<Expression<Func<EventStyle, bool>>>()).Returns(Task.FromResult(new EventStyle()));
            _eventRepository.UpdateAsync(Arg.Any<Event>()).Returns(Task.CompletedTask);

            // Act
            await _eventService.EditAsync(eventDto);

            // Assert
            await _eventRepository.Received(1).UpdateAsync(Arg.Is<Event>(e => e.Title == "Edited Event"));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteEvent()
        {
            // Arrange
            var eventDto = new EventDto { Id = 1, StyleId = 1 }; // Assuming valid event ID and style ID
            _eventStyleRepository.SelectAsync(Arg.Any<Expression<Func<EventStyle, bool>>>()).Returns(Task.FromResult(new EventStyle()));

            // Act
            await _eventService.DeleteAsync(eventDto);

            // Assert
            await _eventRepository.Received(1).DeleteAsync(Arg.Any <Event>());
        }

    }
}
