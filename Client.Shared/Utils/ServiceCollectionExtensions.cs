using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using TherapeutKalendar.Shared.Protos;

namespace Client.Shared.Utils;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterClientSharedServices<P>(this IServiceCollection services) 
        where P : class, IAppConfigurationService
    {
        // Move culture settings to Program.cs or Startup.cs
        
        services.AddSingleton<IAppConfigurationService, P>();
        services.AddScoped<LocalizedTitleService>();
        services.AddScoped<ThemeService>();
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        services.AddSingleton<ClientLoggingInterceptor>();
        services.AddSingleton(services =>
        {
            var config = services.GetRequiredService<IAppConfigurationService>();
            var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
            return GrpcChannel.ForAddress(config.Configuration.BackendUrl, new GrpcChannelOptions
            {
                HttpHandler = httpHandler
            });
        });
        services.AddGrpcClient<TerminService.TerminServiceClient>((provider, options) =>
        {
            var config = provider.GetRequiredService<IAppConfigurationService>();
            options.Address = new Uri(config.Configuration.BackendUrl);
        }).AddInterceptor<ClientLoggingInterceptor>();

        services.AddGrpcClient<TherapistService.TherapistServiceClient>((provider, options) =>
        {
            var config = provider.GetRequiredService<IAppConfigurationService>();
            options.Address = new Uri(config.Configuration.BackendUrl);

        }).AddInterceptor<ClientLoggingInterceptor>();

        return services;
    }
}