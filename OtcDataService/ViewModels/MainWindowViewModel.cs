using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OtcDataService.Models;

namespace OtcDataService.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public HomeViewModel Home { get; } = new();
    public SettingsViewModel Settings { get; } = new();

    [ObservableProperty]
    private AppPage _currentPage = AppPage.Home;

    [ObservableProperty]
    private object? _currentContent;

    public bool IsOnHomePage => CurrentPage == AppPage.Home;
    public bool IsOnSettingsPage => CurrentPage == AppPage.Settings;

    public string CurrentPageLabel => CurrentPage switch
    {
        AppPage.Home => "Home",
        AppPage.Settings => "Settings",
        _ => string.Empty
    };

    public string WindowTitle => AppInfo.WindowTitle;
    public string VersionLabel => AppInfo.VersionLabel;
    public string AppName => AppInfo.Name;

    public MainWindowViewModel()
    {
        Settings.RequestNavigateHome += (_, _) => NavigateHome();
        UpdateCurrentContent();
    }

    [RelayCommand]
    public void NavigateHome()
    {
        CurrentPage = AppPage.Home;
    }

    [RelayCommand]
    public void NavigateSettings()
    {
        Settings.Reload();
        CurrentPage = AppPage.Settings;
    }

    partial void OnCurrentPageChanged(AppPage value)
    {
        UpdateCurrentContent();
        OnPropertyChanged(nameof(IsOnHomePage));
        OnPropertyChanged(nameof(IsOnSettingsPage));
        OnPropertyChanged(nameof(CurrentPageLabel));
    }

    private void UpdateCurrentContent()
    {
        CurrentContent = CurrentPage switch
        {
            AppPage.Home => Home,
            AppPage.Settings => Settings,
            _ => Home
        };
    }
}
