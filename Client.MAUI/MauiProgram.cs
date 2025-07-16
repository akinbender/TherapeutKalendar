using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Client.Shared.Utils;
using Client.MAUI.Services;
namespace Client.MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            //TODO fix
            var supportedCultures = new AppConfiguration().SupportedCultures;
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            builder.Services.RegisterClientSharedServices<AppConfigurationService>();
            return builder.Build();
	}
}
