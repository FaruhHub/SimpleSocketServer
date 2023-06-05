using Microsoft.Extensions.DependencyInjection;

namespace SocketClient.Registrations
{
    public static class SocketClientRegistrations
    {
        public static IServiceCollection AddSocketClientConfigurations(this IServiceCollection services)
        {
            services.AddScoped<ISocketClient, SocketClient>();

            return services;
        }
    }
}
