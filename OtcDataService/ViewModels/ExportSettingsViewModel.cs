using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OtcDataService.Services;

namespace OtcDataService.ViewModels;

public partial class ExportSettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _salesLookbackDays;

    [ObservableProperty]
    private int _documentIntervalDays;

    [ObservableProperty]
    private string _outputFolder = string.Empty;

    [ObservableProperty]
    private string? _statusMessage;

    public ExportSettingsViewModel()
    {
        LoadFromConfiguration();
    }

    public void LoadFromConfiguration()
    {
        var config = AppServices.Configuration.Current;
        SalesLookbackDays = config.SalesLookbackDays;
        DocumentIntervalDays = config.DocumentIntervalDays;
        OutputFolder = config.OutputFolder;
        StatusMessage = null;
    }

    [RelayCommand]
    private async Task BrowseOutputFolderAsync(IStorageProvider? storageProvider)
    {
        if (storageProvider is null)
        {
            return;
        }

        var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Output Folder",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            OutputFolder = folders[0].Path.LocalPath;
        }
    }

    public bool SaveToConfiguration(out string? errorMessage)
    {
        if (SalesLookbackDays <= 0)
        {
            errorMessage = "Sales lookback days must be greater than zero.";
            StatusMessage = errorMessage;
            return false;
        }

        if (DocumentIntervalDays <= 0)
        {
            errorMessage = "Document interval days must be greater than zero.";
            StatusMessage = errorMessage;
            return false;
        }

        if (string.IsNullOrWhiteSpace(OutputFolder))
        {
            errorMessage = "Output folder is required.";
            StatusMessage = errorMessage;
            return false;
        }

        AppServices.Configuration.Update(config =>
        {
            config.SalesLookbackDays = SalesLookbackDays;
            config.DocumentIntervalDays = DocumentIntervalDays;
            config.OutputFolder = OutputFolder.Trim();
        });

        StatusMessage = "Export settings saved.";
        errorMessage = null;
        AppServices.Log.Info("Export settings saved.");
        return true;
    }
}
