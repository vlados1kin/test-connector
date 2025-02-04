using TestConnector.API.Middlewares;
using TestConnector.Domain.Interfaces;
using TestConnector.Infrastructure.TestConnectors;

namespace TestConnector.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureRestTestConnector(this IServiceCollection services)
    {
        services.AddScoped<IRestTestConnector, RestTestConnector>();
        services.AddHttpClient<IRestTestConnector, RestTestConnector>();

        return services;
    }

    public static IServiceCollection ConfigureExceptionHandlingMiddleware(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();

        return services;
    }
}