using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.Presentation.Areas.Authentication.Controllers;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleLNU.Tests
{
    public class RestorePasswordControllerTests
    {
        private readonly IAuthService _authService;
        private readonly RestorePasswordController _controller;

        public RestorePasswordControllerTests()
        {
            _authService = Substitute.For<IAuthService>();
            _controller = new RestorePasswordController(_authService);
        }

        [Fact]
        public void ForgotPasswordForm_Get_ReturnsView()
        {
            // Act
            var result = _controller.ForgotPasswordForm();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task ResetPasswordForm_Get_ReturnsViewWithResetPasswordDto()
        {
            // Arrange
            var email = "test@example.com";
            var token = "some_token";

            // Act
            var result = _controller.ResetPasswordForm(email, token) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<ResetPasswordDto>();
            var model = result.Model as ResetPasswordDto;
            model.Email.Should().Be(email);
            model.Token.Should().Be(token);
        }
    }
}
