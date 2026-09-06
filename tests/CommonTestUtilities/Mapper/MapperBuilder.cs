using AutoMapper;
using BarberBoss.Application.AutoMapper;

namespace CommonTestUtilities.Mapper;

public static class MapperBuilder
{
    public static IMapper Build()
    {
        var configuration = new MapperConfiguration(options => options.AddProfile(new AutoMapping()));

        return configuration.CreateMapper();
    }
}
