using Xunit;
using FluentAssertions;

public class AuthTests
{
    [Fact]
    public void Password_Should_Hash_And_Verify_Correctly()
    {
        var password = "Admin@123";

        var hash = BCrypt.Net.BCrypt.HashPassword(password);

        var result = BCrypt.Net.BCrypt.Verify(password, hash);

        result.Should().BeTrue();
    }
}