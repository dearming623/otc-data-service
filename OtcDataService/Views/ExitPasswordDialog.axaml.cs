using Avalonia.Controls;
using Avalonia.Input;
using OtcDataService.ViewModels;

namespace OtcDataService.Views;

public partial class ExitPasswordDialog : Window
{
    private ExitPasswordDialogViewModel? _viewModel;

    public ExitPasswordDialog()
    {
        InitializeComponent();
        Icon = ExitWindowIconFactory.Create();
        KeyDown += OnKeyDown;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (_viewModel is not null)
        {
            _viewModel.CloseRequested -= OnCloseRequested;
        }

        _viewModel = DataContext as ExitPasswordDialogViewModel;
        if (_viewModel is not null)
        {
            _viewModel.CloseRequested += OnCloseRequested;
        }
    }

    private void OnCloseRequested(bool result)
    {
        Close(result);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || _viewModel is null || !_viewModel.IsPasswordValid)
        {
            return;
        }

        _viewModel.ConfirmCommand.Execute(null);
        e.Handled = true;
    }
}
