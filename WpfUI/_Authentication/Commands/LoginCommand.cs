using Firebase.Auth;
using MVVMEssentials.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using WpfUI._Authentication.ViewModels;

namespace WpfUI._Authentication.Commands;
public class LoginCommand : AsyncCommandBase
{
    private readonly LoginViewModel _loginViewModel;
    private readonly FirebaseAuthProvider _firebaseAuthProvider;
    private readonly AuthenticationMainWindow _authWindow;

    public LoginCommand(LoginViewModel loginViewModel, FirebaseAuthProvider firebaseAuthProvider, AuthenticationMainWindow authWindow)
    {
        _loginViewModel = loginViewModel;
        _firebaseAuthProvider = firebaseAuthProvider;
        _authWindow = authWindow;
    }

    protected override async Task ExecuteAsync(object parameter)
    {
        try {
            await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(
                _loginViewModel.Email,
                _loginViewModel.Password);
            MessageBox.Show("Successfully Logged in.", "Login Successful", MessageBoxButton.OK);
            _authWindow._isLoggedIn = true;
            _authWindow.Close();

        }
        catch (Exception ex) {
            if (ex.Message.Contains("EMAIL_NOT_FOUND")) {
                MessageBox.Show("This email is not registered. Confirm email and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            if (ex.Message.Contains("INVALID_PASSWORD")) {
                MessageBox.Show("Invalid Password. Confirm password and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else {
                MessageBox.Show("Could not login. Confirm eamil and password and try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
