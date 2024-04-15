using Microsoft.AspNetCore.Identity;
using NSubstitute;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.BusinessLogic.Services;
using ScheduleLNU.DataAccess.Entities;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using NSubstitute.ReceivedExtensions;

namespace ScheduleLNU.Tests
{
    public class RegisterServiceTests
    {
        private readonly ICookieService _cookieService;
        private readonly UserManager<Student> _userManager;
        private readonly RegisterService _registerService;

        public RegisterServiceTests()
        {
            _cookieService = Substitute.For<ICookieService>();
            _userManager = Substitute.For<UserManager<Student>>(Substitute.For<IUserStore<Student>>(), null, null, null, null, null, null, null, null);
            _registerService = new RegisterService(_cookieService, _userManager);
        }

        [Fact]
        public async Task RegisterAsync_WhenRegistrationSuccessful_ShouldSetCookiesAndReturnSuccessResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password"
            };
            var user = new Student { Id = "123", UserName = registerDto.Email, Email = registerDto.Email, NormalizedUserName = $"{registerDto.FirstName} {registerDto.LastName}" };
            _userManager.CreateAsync(Arg.Any<Student>(), Arg.Any<string>()).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _registerService.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            await _cookieService.ReceivedWithAnyArgs(1).SetCookies();
        }

        [Fact]
        public async Task RegisterAsync_WhenRegistrationFails_ShouldNotSetCookiesAndReturnFailureResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password"
            };
            _userManager.CreateAsync(Arg.Any<Student>(), Arg.Any<string>()).Returns(Task.FromResult(IdentityResult.Failed()));

            // Act
            var result = await _registerService.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeFalse();
            await _cookieService.DidNotReceiveWithAnyArgs().SetCookies();
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyExists_ShouldNotSetCookiesAndReturnFailureResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password"
            };
            _userManager.CreateAsync(Arg.Any<Student>(), Arg.Any<string>()).Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail" })));

            // Act
            var result = await _registerService.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeFalse();
            await _cookieService.DidNotReceiveWithAnyArgs().SetCookies();
        }

        [Fact]
        public async Task RegisterAsync_WhenPasswordIsTooWeak_ShouldNotSetCookiesAndReturnFailureResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "weak"
            };
            _userManager.CreateAsync(Arg.Any<Student>(), Arg.Any<string>()).Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "PasswordTooWeak" })));

            // Act
            var result = await _registerService.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeFalse();
            await _cookieService.DidNotReceiveWithAnyArgs().SetCookies();
        }
    }
}
