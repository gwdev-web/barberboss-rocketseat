using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Users;
using Moq;

namespace CommonTestUtilities.Repositories;

public class UsersReadOnlyRepositoryBuilder
{
    private readonly Mock<IUsersReadOnlyRepository> _repository = new();

    public UsersReadOnlyRepositoryBuilder ExistsUserWithEmail(string email)
    {
        _repository.Setup(repository => repository.ExistsUserWithEmail(email)).ReturnsAsync(true);

        return this;
    }

    public UsersReadOnlyRepositoryBuilder GetByEmail(User? user)
    {
        _repository.Setup(repository => repository.GetByEmail(It.IsAny<string>())).ReturnsAsync(user);

        return this;
    }

    public UsersReadOnlyRepositoryBuilder GetById(User? user)
    {
        _repository.Setup(repository => repository.GetById(It.IsAny<Guid>())).ReturnsAsync(user);

        return this;
    }

    public IUsersReadOnlyRepository Build() => _repository.Object;
}

public class UsersWriteOnlyRepositoryBuilder
{
    private readonly Mock<IUsersWriteOnlyRepository> _repository = new();

    public UsersWriteOnlyRepositoryBuilder Delete(Guid id, bool result)
    {
        _repository.Setup(repository => repository.Delete(id)).ReturnsAsync(result);

        return this;
    }

    public IUsersWriteOnlyRepository Build() => _repository.Object;
}

public class UsersUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IUsersUpdateOnlyRepository> _repository = new();

    public UsersUpdateOnlyRepositoryBuilder GetById(User? user)
    {
        _repository.Setup(repository => repository.GetById(It.IsAny<Guid>())).ReturnsAsync(user);

        return this;
    }

    public IUsersUpdateOnlyRepository Build() => _repository.Object;
}
