using MVVMEssentials.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using WpfUI._Authentication.Stores;
using WpfUI._Authentication.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WpfUI._Authentication.Commands;
public class LoginCommand : AsyncCommandBase
{
    private readonly LoginViewModel _loginViewModel;
    private readonly AuthenticationStore _authenticationStore;
    private readonly AuthenticationMainWindow _authWindow;

   


    public LoginCommand(LoginViewModel loginViewModel, AuthenticationStore authenticationStore, AuthenticationMainWindow authWindow)
    {
        _loginViewModel = loginViewModel;
        _authenticationStore = authenticationStore;
        _authWindow = authWindow;
    }
    
    EdtAuthorization _edtAuth = new EdtAuthorization();

    protected override async Task ExecuteAsync(object parameter)
    {
        try {
            var firebaseAuthLink = await _authenticationStore.Login(_loginViewModel, _loginViewModel.Email, _loginViewModel.Password);

        }
        catch (Exception ex) {
            if (ex.Message.Contains("EMAIL_NOT_FOUND")) {
                MessageBox.Show("This email is not registered. Confirm email and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else if (ex.Message.Contains("INVALID_PASSWORD")) {
                MessageBox.Show("Invalid Password. Confirm password and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else {
                MessageBox.Show("Could not login. Confirm email and password and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
