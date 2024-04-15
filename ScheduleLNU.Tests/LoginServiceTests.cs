using Microsoft.AspNetCore.Identity;
using NSubstitute;
using ScheduleLNU.BusinessLogic.Constants;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.BusinessLogic.Services;
using ScheduleLNU.DataAccess.Entities;
using ScheduleLNU.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq.Expressions;

namespace ScheduleLNU.Tests
{
    public class LoginServiceTests
    {
        private readonly ICookieService _cookieService;
        private readonly IRepository<Student> _studentRepository;
        private readonly UserManager<Student> _userManager;
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            _cookieService = Substitute.For<ICookieService>();
            _studentRepository = Substitute.For<IRepository<Student>>();
            _userManager = Substitute.For<UserManager<Student>>(Substitute.For<IUserStore<Student>>(), null, null, null, null, null, null, null, null);
            _loginService = new LoginService(_cookieService, _studentRepository, _userManager);
        }

        [Fact]
        public async Task LogInAsync_WhenLoginSuccessful_ShouldSetCookiesAndReturnTrue()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };
            var user = new Student { Id = "123", SelectedTheme = new Theme { FontSize = "12", Font = "Arial", BackColor = "White", ForeColor = "Black" } };
            _studentRepository.SelectAsync(Arg.Any<Expression<Func<Student, bool>>>(), Arg.Any<Expression<Func<Student, object>>[]>())
                .Returns(Task.FromResult(user));
            _userManager.CheckPasswordAsync(user, loginDto.Password).Returns(true);

            // Act
            var result = await _loginService.LogInAsync(loginDto);

            // Assert
            result.Should().BeTrue();
            await _cookieService.Received(1).SetCookies(("studentId", user.Id));
            _cookieService.Received(1).SetSessionData(
                (ThemeConstants.FontSizeKey, user.SelectedTheme.FontSize),
                (ThemeConstants.FontFamilyKey, user.SelectedTheme.Font),
                (ThemeConstants.BackColorKey, user.SelectedTheme.BackColor),
                (ThemeConstants.ForeColorKey, user.SelectedTheme.ForeColor));
        }

        [Fact]
        public async Task LogInAsync_WhenLoginUnsuccessful_ShouldNotSetCookiesAndReturnFalse()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };
            var user = new Student { Id = "123" };
            _studentRepository.SelectAsync(Arg.Any<Expression<Func<Student, bool>>>(), Arg.Any<Expression<Func<Student, object>>[]>())
                .Returns(Task.FromResult(user));
            _userManager.CheckPasswordAsync(user, loginDto.Password).Returns(false);

            // Act
            var result = await _loginService.LogInAsync(loginDto);

            // Assert
            result.Should().BeFalse();
            await _cookieService.DidNotReceiveWithAnyArgs().SetCookies();
            _cookieService.DidNotReceiveWithAnyArgs().SetSessionData();
        }

        [Fact]
        public async Task LogInAsync_WhenUserNotFound_ShouldReturnFalseAndNotSetCookies()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "password"
            };
            _studentRepository.SelectAsync(Arg.Any<Expression<Func<Student, bool>>>(), Arg.Any<Expression<Func<Student, object>>[]>())
                .Returns(Task.FromResult<Student>(null));

            // Act
            var result = await _loginService.LogInAsync(loginDto);

            // Assert
            result.Should().BeFalse();
            await _cookieService.DidNotReceiveWithAnyArgs().SetCookies();
            _cookieService.DidNotReceiveWithAnyArgs().SetSessionData();
        }

        [Fact]
        public async Task LogInAsync_WhenSelectedThemeIsNull_ShouldUseDefaultThemeForCookies()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };
            var user = new Student { Id = "123", SelectedTheme = null };
            _studentRepository.SelectAsync(Arg.Any<Expression<Func<Student, bool>>>(), Arg.Any<Expression<Func<Student, object>>[]>())
                .Returns(Task.FromResult(user));
            _userManager.CheckPasswordAsync(user, loginDto.Password).Returns(true);

            // Act
            await _loginService.LogInAsync(loginDto);

            // Assert
            await _cookieService.Received(1).SetCookies(("studentId", user.Id));
            _cookieService.Received(1).SetSessionData(
                (ThemeConstants.FontSizeKey, ThemeConstants.DefaultTheme.FontSize),
                (ThemeConstants.FontFamilyKey, ThemeConstants.DefaultTheme.Font),
                (ThemeConstants.BackColorKey, ThemeConstants.DefaultTheme.BackColor),
                (ThemeConstants.ForeColorKey, ThemeConstants.DefaultTheme.ForeColor));
        }

        [Fact]
        public async Task LogInAsync_WhenUserIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };
            _studentRepository.SelectAsync(Arg.Any<Expression<Func<Student, bool>>>(), Arg.Any<Expression<Func<Student, object>>[]>())
                .Returns(Task.FromResult<Student>(null));

            _userManager.CheckPasswordAsync(Arg.Any<Student>(), loginDto.Password).Returns(Task.FromResult<bool>(true));

            // Act
            Func<Task> act = async () => await _loginService.LogInAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
            await _cookieService.DidNotReceiveWithAnyArgs().SetCookies();
            _cookieService.DidNotReceiveWithAnyArgs().SetSessionData();
        }
    }
}
