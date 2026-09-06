using System.Security.Claims;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using BarberBoss.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.Services.LoggedUser;

public class LoggedUser : ILoggedUser
{
    private readonly BarberBossDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggedUser(BarberBossDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User> Get()
    {
        var principal = _httpContextAccessor.HttpContext?.User
            ?? throw new ForbiddenException();

        // Dependendo do handler, o claim chega como URI (ClaimTypes.Sid) ou como "sid".
        var claim = principal.FindFirst(ClaimTypes.Sid) ?? principal.FindFirst("sid");

        if (claim is null || Guid.TryParse(claim.Value, out var userId) == false)
            throw new ForbiddenException();

        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == userId)
            ?? throw new NotFoundException(ResourceMessagesException.USER_NOT_FOUND);
    }
}
