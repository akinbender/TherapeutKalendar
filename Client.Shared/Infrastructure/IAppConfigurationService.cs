namespace Client.Shared.Infrastructure
{
    public interface IAppConfigurationService
    {
        AppConfiguration Configuration { get; }
        Task InitializeAsync();
        Task SaveConfigurationAsync();
        Task ResetToDefaultsAsync();
    }
}
