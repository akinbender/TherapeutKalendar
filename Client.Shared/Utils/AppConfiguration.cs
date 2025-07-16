namespace Client.Shared.Utils;

public class AppConfiguration
{
    public Theme Theme { get; set; } = Theme.Auto;
    public string[] SupportedCultures { get; } = ["en-US", "de-DE"];
    public string Language { get; set; } = "en-US";
    public string BackendUrl { get; set; } = "http://localhost:8081";
    public bool AnalyticsEnabled { get; set; } = true;
    public DateTime? LastUpdated { get; set; }
}