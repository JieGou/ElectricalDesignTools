using MVVMEssentials.Commands;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using WpfUI._Authentication.OfflineLicense;
using WpfUI._Authentication.Stores;
using WpfUI._Authentication.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WpfUI._Authentication.Commands;
public class LoginOfflineCommand : AsyncCommandBase
{
    private readonly LoginViewModel _loginViewModel;
    private readonly AuthenticationStore _authenticationStore;
    private readonly AuthenticationMainWindow _authWindow;

   


    public LoginOfflineCommand(LoginViewModel loginViewModel, AuthenticationStore authenticationStore, AuthenticationMainWindow authWindow)
    {
        _loginViewModel = loginViewModel;
        _authenticationStore = authenticationStore;
        _authWindow = authWindow;
    }
    
    EdtAuthDbManager _edtAuth = new EdtAuthDbManager();

    protected override async Task ExecuteAsync(object parameter)
    {
        try {
            var offLineLoginResult = OfflineLicenseManager.ValidateLicense(_loginViewModel.Email, _loginViewModel.Password);
            if (offLineLoginResult.Item1) {
                _authenticationStore.LoginOffline(offLineLoginResult.Item2);
            }
            
            
        }
        catch (Exception ex) {
            MessageBox.Show("Could not login. Confirm email and password and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
