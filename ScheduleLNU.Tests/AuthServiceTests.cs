using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NSubstitute;
using ScheduleLNU.BusinessLogic.DTOs;
using ScheduleLNU.BusinessLogic.Services;
using ScheduleLNU.BusinessLogic.Services.EmailService;
using ScheduleLNU.BusinessLogic.Services.Interfaces;
using ScheduleLNU.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleLNU.Tests
{
    public class AuthServiceTests
    {
        private readonly UserManager<Student> userManager;
        private readonly IEmailSender emailSender;
        private readonly AuthService authService;

        public AuthServiceTests()
        {
            userManager = Substitute.For<UserManager<Student>>(Substitute.For<IUserStore<Student>>(), null, null, null, null, null, null, null, null);
            emailSender = Substitute.For<IEmailSender>();
            authService = new AuthService(userManager, emailSender);
        }

        [Fact]
        public async Task SendResetTokenAsync_WhenUserFound_ShouldSendResetEmail()
        {
            // Arrange
            var email = "test@example.com";
            var user = new Student { Email = email };
            userManager.FindByEmailAsync(email).Returns(Task.FromResult(user));

            // Act
            await authService.SendResetTokenAsync(email);

            // Assert
            await emailSender.Received(1).SendEmailAsync(Arg.Any<Message>());
        }

        [Fact]
        public async Task SendResetTokenAsync_WhenUserNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            var email = "nonexistent@example.com";
            userManager.FindByEmailAsync(email).Returns(Task.FromResult<Student>(null));

            // Act
            Func<Task> act = async () => await authService.SendResetTokenAsync(email);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("User is not found");
            await emailSender.DidNotReceive().SendEmailAsync(Arg.Any<Message>());
        }

        [Fact]
        public async Task ResetPasswordAsync_WhenPasswordsMatch_ShouldResetPassword()
        {
            // Arrange
            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "test@example.com",
                NewPassword = "newPassword",
                ConfirmedPassword = "newPassword",
                Token = "token"
            };
            var user = new Student { Email = resetPasswordDto.Email };
            userManager.FindByEmailAsync(resetPasswordDto.Email).Returns(Task.FromResult(user));
            userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword)
                .Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await authService.ResetPasswordAsync(resetPasswordDto);

            // Assert
            result.Should().BeEquivalentTo(IdentityResult.Success);
        }

        [Fact]
        public async Task ResetPasswordAsync_WhenPasswordsMismatch_ShouldThrowArgumentException()
        {
            // Arrange
            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "test@example.com",
                NewPassword = "newPassword",
                ConfirmedPassword = "differentPassword",
                Token = "token"
            };

            // Act
            Func<Task> act = async () => await authService.ResetPasswordAsync(resetPasswordDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("New password should be equal to confirmed!");
            await userManager.DidNotReceive().ResetPasswordAsync(Arg.Any<Student>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task ResetPasswordAsync_WhenResetFails_ShouldReturnFailureResult()
        {
            // Arrange
            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "test@example.com",
                NewPassword = "newPassword",
                ConfirmedPassword = "newPassword",
                Token = "token"
            };
            var user = new Student { Email = resetPasswordDto.Email };
            userManager.FindByEmailAsync(resetPasswordDto.Email).Returns(Task.FromResult(user));
            userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword)
                .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Reset failed" })));

            // Act
            var result = await authService.ResetPasswordAsync(resetPasswordDto);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.Description.Should().Be("Reset failed");
        }
    }
}
