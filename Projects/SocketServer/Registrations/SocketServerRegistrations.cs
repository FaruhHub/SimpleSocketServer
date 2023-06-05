using Microsoft.Extensions.DependencyInjection;

namespace SocketServer.Registrations;

public static class SocketServerRegistrations
{
    public static IServiceCollection AddSocketServerConfigurations(this IServiceCollection services)
    {
        services.AddScoped<ISocketServer, SocketServer>();

        return services;
    }
}