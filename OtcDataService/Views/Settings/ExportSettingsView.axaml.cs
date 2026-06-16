using Avalonia.Controls;
using Avalonia.Interactivity;
using OtcDataService.ViewModels;

namespace OtcDataService.Views.Settings;

public partial class ExportSettingsView : UserControl
{
    public ExportSettingsView()
    {
        InitializeComponent();
    }

    private async void OnBrowseClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ExportSettingsViewModel viewModel)
        {
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is null)
        {
            return;
        }

        await viewModel.BrowseOutputFolderCommand.ExecuteAsync(topLevel.StorageProvider);
    }
}
