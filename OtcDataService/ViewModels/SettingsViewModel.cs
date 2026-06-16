using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OtcDataService.Models;

namespace OtcDataService.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public DatabaseSettingsViewModel Database { get; } = new();
    public ExportSettingsViewModel Export { get; } = new();

    [ObservableProperty]
    private SettingsSection _selectedSection = SettingsSection.Database;

    [ObservableProperty]
    private string? _statusMessage;

    public bool IsDatabaseSelected => SelectedSection == SettingsSection.Database;
    public bool IsExportSelected => SelectedSection == SettingsSection.Export;

    public string SelectedSectionTitle => SelectedSection switch
    {
        SettingsSection.Database => "Database Settings",
        SettingsSection.Export => "Export Settings",
        _ => "Settings"
    };

    partial void OnSelectedSectionChanged(SettingsSection value)
    {
        OnPropertyChanged(nameof(IsDatabaseSelected));
        OnPropertyChanged(nameof(IsExportSelected));
        OnPropertyChanged(nameof(SelectedSectionTitle));
    }

    public event EventHandler? RequestNavigateHome;

    [RelayCommand]
    private void SelectDatabase() => SelectedSection = SettingsSection.Database;

    [RelayCommand]
    private void SelectExport() => SelectedSection = SettingsSection.Export;

    [RelayCommand]
    private void Save()
    {
        if (!Database.SaveToConfiguration(out var databaseError))
        {
            StatusMessage = databaseError;
            SelectedSection = SettingsSection.Database;
            return;
        }

        if (!Export.SaveToConfiguration(out var exportError))
        {
            StatusMessage = exportError;
            SelectedSection = SettingsSection.Export;
            return;
        }

        StatusMessage = "All settings saved.";
    }

    [RelayCommand]
    private void Cancel()
    {
        Database.LoadFromConfiguration();
        Export.LoadFromConfiguration();
        StatusMessage = "Changes discarded.";
    }

    [RelayCommand]
    private void Back()
    {
        RequestNavigateHome?.Invoke(this, EventArgs.Empty);
    }

    public void Reload()
    {
        Database.LoadFromConfiguration();
        Export.LoadFromConfiguration();
        StatusMessage = null;
    }
}
