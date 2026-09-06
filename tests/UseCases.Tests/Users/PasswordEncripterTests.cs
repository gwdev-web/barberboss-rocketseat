using CommonTestUtilities.Cryptography;
using FluentAssertions;

namespace UseCases.Tests.Users;

public class PasswordEncripterTests
{
    [Fact]
    public void Hash_Is_Different_From_Plain_Password()
    {
        var encripter = PasswordEncripterBuilder.Build();

        var hash = encripter.Encrypt("barberboss123");

        hash.Should().NotBe("barberboss123");
        hash.Should().StartWith("$2");
    }

    [Fact]
    public void Same_Password_Generates_Different_Hashes()
    {
        var encripter = PasswordEncripterBuilder.Build();

        encripter.Encrypt("barberboss123").Should().NotBe(encripter.Encrypt("barberboss123"));
    }

    [Fact]
    public void Verify_Returns_True_For_Correct_Password()
    {
        var encripter = PasswordEncripterBuilder.Build();
        var hash = encripter.Encrypt("barberboss123");

        encripter.Verify("barberboss123", hash).Should().BeTrue();
        encripter.Verify("outra-senha", hash).Should().BeFalse();
    }

    [Fact]
    public void Verify_Returns_False_For_Malformed_Hash()
    {
        var encripter = PasswordEncripterBuilder.Build();

        encripter.Verify("barberboss123", "nao-e-um-hash").Should().BeFalse();
    }
}
