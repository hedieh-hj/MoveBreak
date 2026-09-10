using System.Windows;
using WpfApplication = System.Windows.Application;

namespace MoveBreak.Services;

public sealed class LocalizationService
{
    private ResourceDictionary? _activeDictionary;
    public string CurrentLanguage { get; private set; } = "en";
    public bool IsPersian => CurrentLanguage == "fa";
    public event Action? LanguageChanged;

    public void Apply(string? languageCode)
    {
        var normalized = languageCode is "fa" or "es" ? languageCode : "en";
        var dictionary = new ResourceDictionary
        {
            Source = new Uri($"/MoveBreak;component/Resources/Strings.{normalized}.xaml", UriKind.Relative)
        };

        if (_activeDictionary is not null)
            WpfApplication.Current.Resources.MergedDictionaries.Remove(_activeDictionary);

        WpfApplication.Current.Resources.MergedDictionaries.Add(dictionary);
        _activeDictionary = dictionary;
        CurrentLanguage = normalized;
        LanguageChanged?.Invoke();
    }

    public string Text(string key) => WpfApplication.Current.TryFindResource(key)?.ToString() ?? key;
}
