using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OtcDataService.ViewModels;

public partial class ExitPasswordDialogViewModel : ViewModelBase
{
    public event Action<bool>? CloseRequested;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isPasswordValid;

    partial void OnPasswordChanged(string value)
    {
        IsPasswordValid = value == DateTime.Now.ToString("yyyyMMddHH");
    }

    [RelayCommand]
    private void ClearPassword()
    {
        Password = string.Empty;
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(false);
    }

    [RelayCommand]
    private void Confirm()
    {
        if (!IsPasswordValid)
        {
            return;
        }

        CloseRequested?.Invoke(true);
    }
}
