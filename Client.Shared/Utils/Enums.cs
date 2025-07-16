using System.Resources;
using System.ComponentModel.DataAnnotations;
namespace Client.Shared.Utils;

public static class Enums
{
    public enum Theme
    {
        [Display(Name = nameof(Resources.Theme_Auto), ResourceType = typeof(Resources))]
        Auto,
        [Display(Name = nameof(Resources.Theme_Light), ResourceType = typeof(Resources))]
        Light,
        [Display(Name = nameof(Resources.Theme_Dark), ResourceType = typeof(Resources))]
        Dark
    }

    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name?? value.ToString();
    }

    public static string GetLocalizedDisplayName(this Enum value)
    {
        var rm = new ResourceManager(typeof(Resources));
        var name = value.GetDisplayName();
        var resourceDisplayName = rm.GetString(name);

        return string.IsNullOrWhiteSpace(resourceDisplayName) ? name : resourceDisplayName;
    }
}