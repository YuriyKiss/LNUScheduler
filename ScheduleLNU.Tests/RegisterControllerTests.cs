using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.Presentation.Areas.Authentication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleLNU.Tests
{
    public class RegisterControllerTests
    {
        private readonly IRegisterService _registerService;
        private readonly RegisterController _controller;

        public RegisterControllerTests()
        {
            _registerService = Substitute.For<IRegisterService>();
            _controller = new RegisterController(_registerService);
        }

        [Fact]
        public void Register_Get_ReturnsView()
        {
            // Act
            var result = _controller.Register();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Register_Post_ValidModelAndSuccessfulRegistration_RedirectsToHomePage()
        {
            // Arrange
            var registerDto = new RegisterDto { /* valid register data */ };
            _registerService.RegisterAsync(registerDto).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _controller.Register(registerDto) as RedirectToActionResult;

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Register_Post_ValidModelAndUnsuccessfulRegistration_AddsModelErrorAndReturnsView()
        {
            // Arrange
            var registerDto = new RegisterDto { /* valid register data */ };
            var errors = new List<IdentityError> { new IdentityError { Description = "Error 1" } };
            var identityResult = IdentityResult.Failed(errors.ToArray());
            _registerService.RegisterAsync(registerDto).Returns(Task.FromResult(identityResult));

            // Act
            var result = await _controller.Register(registerDto) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull(); // Assumes default view
            result.Model.Should().Be(registerDto);
            result.ViewData.ModelState.ErrorCount.Should().Be(1);
            result.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Error 1");
        }

        [Fact]
        public async Task Register_Post_InvalidModel_ReturnsViewWithModelErrors()
        {
            // Arrange
            var registerDto = new RegisterDto { /* invalid register data */ };
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Register(registerDto) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNull(); // Assumes default view
            result.Model.Should().Be(registerDto);
            result.ViewData.ModelState.ErrorCount.Should().Be(1);
            result.ViewData.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Email is required");
        }
    }
}
