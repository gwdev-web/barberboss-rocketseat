using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Services.LoggedUser;
using Moq;

namespace CommonTestUtilities.LoggedUser;

public static class LoggedUserBuilder
{
    public static ILoggedUser Build(User user)
    {
        var mock = new Mock<ILoggedUser>();
        mock.Setup(logged => logged.Get()).ReturnsAsync(user);

        return mock.Object;
    }
}
