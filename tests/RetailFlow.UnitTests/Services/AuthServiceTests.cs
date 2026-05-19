using System;
using FluentAssertions;
using Moq;
using RetailFlow.Application.DTOs;
using RetailFlow.Application.Services;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Shared.Helpers;
using Xunit;

namespace RetailFlow.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IRepository<Role>> _roleRepoMock;
        private readonly AuthService _sut;
        private const string JwtSecret = "test-secret-key-that-is-long-enough-for-hmac-sha256";

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _roleRepoMock = new Mock<IRepository<Role>>();
            _sut = new AuthService(_userRepoMock.Object, _roleRepoMock.Object, JwtSecret, 60);
        }

        [Fact]
        public void Register_WithNewEmail_ReturnsAuthResponse()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((User)null);
            _roleRepoMock.Setup(r => r.GetById(1)).Returns(new Role { Id = 1, Name = "Customer" });

            var request = new RegisterRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password1"
            };

            // Act
            var result = _sut.Register(request);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
            result.User.Email.Should().Be("john@example.com");
            result.User.Role.Should().Be("Customer");

            _userRepoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(r => r.SaveChanges(), Times.AtLeastOnce);
        }

        [Fact]
        public void Register_WithExistingEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            _userRepoMock.Setup(r => r.GetByEmail(It.IsAny<string>()))
                         .Returns(new User { Email = "existing@example.com" });

            var request = new RegisterRequest
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "existing@example.com",
                Password = "Password1"
            };

            // Act & Assert
            _sut.Invoking(s => s.Register(request))
                .Should().Throw<InvalidOperationException>()
                .WithMessage("*already registered*");
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var hash = PasswordHelper.HashPassword("Password1");
            var user = new User
            {
                Id = 1,
                Email = "john@example.com",
                PasswordHash = hash,
                RoleId = 1,
                IsActive = true
            };

            _userRepoMock.Setup(r => r.GetByEmail("john@example.com")).Returns(user);
            _roleRepoMock.Setup(r => r.GetById(1)).Returns(new Role { Id = 1, Name = "Customer" });

            // Act
            var result = _sut.Login(new LoginRequest
            {
                Email = "john@example.com",
                Password = "Password1"
            });

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Login_WithWrongPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var hash = PasswordHelper.HashPassword("CorrectPassword1");
            var user = new User
            {
                Id = 1,
                Email = "john@example.com",
                PasswordHash = hash,
                IsActive = true
            };

            _userRepoMock.Setup(r => r.GetByEmail("john@example.com")).Returns(user);

            // Act & Assert
            _sut.Invoking(s => s.Login(new LoginRequest
                {
                    Email = "john@example.com",
                    Password = "WrongPassword1"
                }))
                .Should().Throw<UnauthorizedAccessException>();
        }

        [Fact]
        public void Login_WithInactiveAccount_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "john@example.com",
                PasswordHash = PasswordHelper.HashPassword("Password1"),
                IsActive = false
            };

            _userRepoMock.Setup(r => r.GetByEmail("john@example.com")).Returns(user);

            // Act & Assert
            _sut.Invoking(s => s.Login(new LoginRequest
                {
                    Email = "john@example.com",
                    Password = "Password1"
                }))
                .Should().Throw<UnauthorizedAccessException>();
        }
    }
}
