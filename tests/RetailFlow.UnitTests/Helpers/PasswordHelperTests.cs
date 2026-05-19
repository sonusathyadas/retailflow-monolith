using FluentAssertions;
using RetailFlow.Shared.Helpers;
using Xunit;

namespace RetailFlow.UnitTests.Helpers
{
    public class PasswordHelperTests
    {
        [Fact]
        public void HashPassword_ProducesDifferentHashesForSameInput()
        {
            var hash1 = PasswordHelper.HashPassword("MyPassword1");
            var hash2 = PasswordHelper.HashPassword("MyPassword1");

            // Salt is random so hashes must differ
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            var hash = PasswordHelper.HashPassword("MyPassword1");
            PasswordHelper.VerifyPassword("MyPassword1", hash).Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            var hash = PasswordHelper.HashPassword("MyPassword1");
            PasswordHelper.VerifyPassword("WrongPassword", hash).Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_EmptyPassword_ReturnsFalse()
        {
            var hash = PasswordHelper.HashPassword("MyPassword1");
            PasswordHelper.VerifyPassword("", hash).Should().BeFalse();
        }
    }
}
