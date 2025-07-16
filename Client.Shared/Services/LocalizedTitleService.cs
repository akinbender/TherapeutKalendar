using Microsoft.Extensions.Localization;

namespace Client.Shared.Services
{
    public class LocalizedTitleService(IStringLocalizer<Resources> localizer) : IDisposable
    {
        private readonly IStringLocalizer<Resources> _localizer = localizer;
        private string _baseTitleKey = string.Empty;
        private object[] _titleArguments = Array.Empty<object>();

        public event Action? OnTitleChanged;

        public string CurrentTitle =>
            string.IsNullOrEmpty(_baseTitleKey)
                ? string.Empty
                : _localizer[_baseTitleKey, _titleArguments];

        public void SetTitle(string titleKey, params object[] arguments)
        {
            _baseTitleKey = titleKey;
            _titleArguments = arguments;
            OnTitleChanged?.Invoke();
        }

        public void Dispose()
        {
            OnTitleChanged = null;
        }
    }
}
